from ultralytics import YOLO
import cv2
import uuid
import datetime
from DB_connection import (
    get_logged_in_user,
    log_error,
    insert_product_detail,
    upsert_product_data
)

model = YOLO("C:/Users/서민재/runs/detect/train7/weights/best.pt")  # 전체 경로

user_name = get_logged_in_user()
today = datetime.date.today()
year  = today.year
month = today.month + 1
if month > 12:
    month, year = 1
    year += 1
delivery_due = f"{year}-{month:02d}-{today.day:02d}"

product_name      = input("제품명: ").strip()
product_info      = input("제품 설명: ").strip()
required_quantity = int(input("요청 수량: ").strip())

processed_ids = set()

for result in model.track(source=0, stream=True, conf=0.3):
    frame = result.orig_img
    
    for box in result.boxes:
        if box.id is None:
            continue
        tid = int(box.id)
        if tid in processed_ids:
            continue
        processed_ids.add(tid)

        x1, y1, x2, y2 = [int(v) for v in box.xyxy[0]]
        cx, cy = frame.shape[1] // 2, frame.shape[0] // 2
        bx, by = (x1 + x2) // 2, (y1 + y2) // 2
        if abs(bx - cx) > 50 or abs(by - cy) > 50:
            continue

        cls_id = int(box.cls)
        name = result.names[cls_id]
        conf = box.conf.item()

        if name == "cap_ok":
            is_defect, defect_type = False, None
        elif name == "cap_ng_dent":
            is_defect, defect_type = True, "dent"
        elif name == "cap_ng_scratch":
            is_defect, defect_type = True, "scratch"
        else:
            continue

        serial_number = str(uuid.uuid4())
        print(f"[DEBUG] insert_product_detail: {serial_number}, {user_name}, {product_name}, {is_defect}")
        upsert_product_data(product_name, product_info, delivery_due, required_quantity, is_defect)
        insert_product_detail(serial_number, user_name, product_name, is_defect)
        if is_defect:
            log_error(serial_number, defect_type)

        color = (0, 0, 255) if is_defect else (255, 0, 0)
        label = f"{'불량' if is_defect else '정상'} ({conf:.2f})"
        cv2.rectangle(frame, (x1,y1), (x2,y2), color, 2)
        cv2.putText(frame, label, (x1, y1-10), cv2.FONT_HERSHEY_SIMPLEX, 0.6, color, 2)

    cv2.imshow("Track & Detect", frame)
    if cv2.waitKey(1) & 0xFF == 27:
        break
    
cv2.destroyAllWindows()
