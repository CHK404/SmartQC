from ultralytics import YOLO
import cv2
import mysql.connector

model = YOLO("C:/Users/서민재/runs/detect/train7/weights/best.pt")  # 전체 경로
cap = cv2.VideoCapture(0)

db_config = {
    'host': '15.164.48.30',
    'port': 3306,
    'user': 'dbchk',
    'password': 'codingon2751',
    'database': 'SmartQC'
}

while True:
    ret, frame = cap.read()
    if not ret:
        break

    frame_height, frame_width = frame.shape[:2]
    cx_center, cy_center = frame_width // 2, frame_height // 2
    center_tolerance = 50  # 중앙 판별 범위

    results = model.predict(frame, conf=0.3)
    annotated_frame = results[0].plot()

    for box in results[0].boxes:
        x1, y1, x2, y2 = box.xyxy[0].tolist()
        cls_id = int(box.cls.item())
        conf = box.conf.item()
        name = results[0].names[cls_id]

        # 바운딩 박스 중심 좌표
        bbox_cx = (x1 + x2) / 2
        bbox_cy = (y1 + y2) / 2

        # 중앙 판별
        in_center = (
            abs(bbox_cx - cx_center) < center_tolerance and
            abs(bbox_cy - cy_center) < center_tolerance
        )

        if not in_center:
            continue  # 중앙에 없으면 무시

        # 불량/정상 분류 및 색상 선택
        if name == "cap_ng_dent":
            label = f"불량 - 구부러짐 (conf: {conf:.2f}) ❌"
            color = (0, 0, 255)  # 빨간색
            print("❌ 병뚜껑 불량 - 구부러짐 (conf:", round(conf, 2), ")")

        elif name == "cap_ng_scratch":
            label = f"불량 - 스크래치 (conf: {conf:.2f}) ❌"
            color = (0, 0, 255)  # 빨간색
            print("❌ 병뚜껑 불량 - 스크래치 (conf:", round(conf, 2), ")")

        elif name == "cap_ok":
            label = f"정상 (conf: {conf:.2f}) ✅"
            color = (255, 0, 0)  # 파란색
            print("✅ 병뚜껑 정상 (conf:", round(conf, 2), ")")

        else:
            continue  # 예외 클래스 무시

        # 강조된 박스 및 라벨 출력
        cv2.rectangle(annotated_frame, (int(x1), int(y1)), (int(x2), int(y2)), color, 2)
        cv2.putText(annotated_frame, label, (int(x1), int(y1) - 10),
                    cv2.FONT_HERSHEY_SIMPLEX, 0.6, color, 2)

    cv2.imshow("Detect", annotated_frame)
    key = cv2.waitKey(1) & 0xFF
    if key == 27:  # ESC 누르면 종료
        break

cap.release()
cv2.destroyAllWindows()