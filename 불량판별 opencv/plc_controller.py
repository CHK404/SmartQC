from ultralytics import YOLO
import cv2
import time
from pymodbus.client.sync import ModbusTcpClient

model = YOLO("C:/Users/서민재/runs/detect/train7/weights/best.pt")
#plc연동 로직
client = ModbusTcpClient('192.168.0.10', port=502)#이건 현수한테 물어보고 숫자만 바꾸면 됨
client.connect()
#이부분도 메모리 주소 plc로직 배선한 릴레이 보고 현수랑 맞춰서
ADDR_STOP = 10     # %MX10
ADDR_START = 11    # %MX11
ADDR_OK = 5        # %MX5
ADDR_NG = 6        # %MX6
def trigger(address):
    client.write_coil(address, True, unit=1)
    time.sleep(0.3)
    client.write_coil(address, False, unit=1)
cap = cv2.VideoCapture(0)
#펄스 신호 중복 보내기 방지
object_detected = False
while True:
    ret, frame = cap.read()
    if not ret:
        break
    frame_height, frame_width = frame.shape[:2]
    cx_center, cy_center = frame_width // 2, frame_height // 2
    center_tolerance = 50  # 중앙 판별 범위
    results = model.predict(frame, conf=0.3)
    annotated_frame = results[0].plot()
    if len(results[0].boxes) == 0:
        object_detected = False  # 박스 없으면 초기화
        cv2.imshow("Detect", annotated_frame)
        if cv2.waitKey(1) & 0xFF == 27:
            break
        continue
    for box in results[0].boxes:
        x1, y1, x2, y2 = box.xyxy[0].tolist()
        cls_id = int(box.cls.item())
        conf = box.conf.item()
        name = results[0].names[cls_id]
        bbox_cx = (x1 + x2) / 2
        bbox_cy = (y1 + y2) / 2
        in_center = (
            abs(bbox_cx - cx_center) < center_tolerance and
            abs(bbox_cy - cy_center) < center_tolerance
        )
        if not in_center:
            continue  # 중앙 아니면 무시
        # ==== 원래 코드: 화면 표시 및 로그 출력 ====
        if name == "cap_ng_dent":
            label = f"불량 - 구부러짐 (conf: {conf:.2f}) :x:"
            color = (0, 0, 255)
            print(":x: 병뚜껑 불량 - 구부러짐 (conf:", round(conf, 2), ")")
        elif name == "cap_ng_scratch":
            label = f"불량 - 스크래치 (conf: {conf:.2f}) :x:"
            color = (0, 0, 255)
            print(":x: 병뚜껑 불량 - 스크래치 (conf:", round(conf, 2), ")")
        elif name == "cap_ok":
            label = f"정상 (conf: {conf:.2f}) :흰색_확인_표시:"
            color = (255, 0, 0)
            print(":흰색_확인_표시: 병뚜껑 정상 (conf:", round(conf, 2), ")")
        else:
            continue
        cv2.rectangle(annotated_frame, (int(x1), int(y1)), (int(x2), int(x2)), color, 2)
        cv2.putText(annotated_frame, label, (int(x1), int(y1) - 10),
                    cv2.FONT_HERSHEY_SIMPLEX, 0.6, color, 2)
        #여기부터 추가된 PLC 연동 로직(신호 보내기)
        if not object_detected:
            trigger(ADDR_STOP)     # 컨베이어 정지
            time.sleep(1.0)        # 안정화 대기
            if name == "cap_ok":
                trigger(ADDR_OK)
            elif name in ["cap_ng_dent", "cap_ng_scratch"]:
                trigger(ADDR_NG)
            trigger(ADDR_START)    # 컨베이어 재시작
            object_detected = True  # 중복 방지
    # ==== 마지막 화면 출력 ====
    cv2.imshow("Detect", annotated_frame)
    if cv2.waitKey(1) & 0xFF == 27:  # ESC로 종료
        break
cap.release()
cv2.destroyAllWindows()
client.close()