from ultralytics import YOLO
import os
import cv2
import uuid
import time
import datetime
from .DB_connection import (log_error, insert_product_detail, upsert_product_data, product_exists)

class DefectDetector:
    def __init__(self, user_name):
        weights_path = os.path.join(os.path.dirname(__file__), "models", "best.pt")
        self.model = YOLO(weights_path)
        self.user_name = user_name

    def run(self):
        today = datetime.date.today()
        year, month = today.year, today.month + 1
        if month > 12:
            month = 1
            year += 1
        delivery_due = f"{year}-{month:02d}-{today.day:02d}"

        product_name = input("제품명: ").strip()
        if product_exists(product_name):
            product_info = ""
            required_quantity = 0
        else:
            product_info = input("제품 설명: ").strip()
            required_quantity = int(input("요청 수량: ").strip())

        processed_times = {}
        DETECT_COOLDOWN = 5.0

        for result in self.model.track(source=0, stream=True, conf=0.3):
            frame = result.orig_img
            now = time.time()
            cx, cy = frame.shape[1] // 2, frame.shape[0] // 2

            for box in result.boxes:
                if box.id is None:
                    continue
                tid = int(box.id)
                x1, y1, x2, y2 = [int(v) for v in box.xyxy[0]]
                bx, by = (x1 + x2) // 2, (y1 + y2) // 2
                if abs(bx - cx) > 100 or abs(by - cy) > 100:
                    continue

                cls_id = int(box.cls)
                name = result.names[cls_id]
                conf = box.conf.item()
                color = (0, 0, 255) if name.startswith("cap_ng_") else (255, 0, 0)
                label = f"{'불량' if name!='cap_ok' else '정상'} ({conf:.2f})"
                cv2.rectangle(frame, (x1, y1), (x2, y2), color, 2)
                cv2.putText(frame, label, (x1, y1 - 10),
                            cv2.FONT_HERSHEY_SIMPLEX, 0.6, color, 2)

                last = processed_times.get(tid, 0.0)
                if now - last < DETECT_COOLDOWN:
                    continue

                processed_times[tid] = now
                serial_number = str(uuid.uuid4())
                is_defect = (name != "cap_ok")

                upsert_product_data(product_name, product_info, delivery_due, required_quantity, is_defect)
                insert_product_detail(serial_number, self.user_name, product_name, is_defect)
                if is_defect:
                    log_error(serial_number, name.replace("cap_ng_", ""))

                print(f"DB_Insert: {serial_number}, "f"{self.user_name}, "f"{product_name}, "f"{is_defect}")

            cv2.imshow("Track & Detect", frame)
            if cv2.waitKey(1) & 0xFF == 27:
                break

        cv2.destroyAllWindows()

