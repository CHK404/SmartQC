import cv2
import numpy as np
from insightface.app import FaceAnalysis
from numpy.linalg import norm
import mysql.connector
import time

db_config = {
    'host': '15.164.48.30',
    'port': 3306,
    'user': 'dbchk',
    'password': 'codingon2751',
    'database': 'SmartQC'
}

app = FaceAnalysis(providers=['CPUExecutionProvider'])
app.prepare(ctx_id=0, det_size=(640, 640))

def l2_normalize(x):
    return x / norm(x)

def insert_user_embedding(user_name, face_id):
    conn = mysql.connector.connect(**db_config)
    cursor = conn.cursor()
    face_id_bytes = face_id.tobytes()
    
    if face_id_bytes is None or len(face_id_bytes) == 0:
        print("[ERROR] 임베딩이 비어 있습니다. 저장하지 않음.")
        return
        
    sql = "INSERT INTO UserData (UserName, Department, Position, FaceID) VALUES (%s,%s,%s, %s)"
    data = (user_info["name"],user_info["department"], user_info["position"], face_id_bytes)
    cursor.execute(sql,data)
    conn.commit()
    cursor.close()
    conn.close()

def set_login_flag(user_name, flag=1):
    conn = mysql.connector.connect(**db_config)
    cursor = conn.cursor()
    sql = "UPDATE UserData SET IsLoggedIn = %s WHERE UserName = %s"
    cursor.execute(sql, (flag, user_name))
    conn.commit()
    cursor.close()
    conn.close()

def capture_and_register(user_name, capture_time=10, delay=0.5):
    cap = cv2.VideoCapture(1)
    if not cap.isOpened():
        print("[ERROR] 카메라를 켜주세요.")
        return

    start_time = time.time()
    embeddings = []

    print(f"[INFO] {capture_time}초간 얼굴 등록을 시작합니다. 화면을 바라봐주세요.")
    while time.time() - start_time < capture_time:
        ret, frame = cap.read()
        if not ret:
            continue

        faces = app.get(frame)
        if len(faces) > 0:
            emb = l2_normalize(faces[0].embedding)
            embeddings.append(emb)
            cv2.putText(frame, "Face Captured", (20, 50), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0), 2)
        else:
            cv2.putText(frame, "No Face Detected", (20, 50), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 0, 255), 2)

        cv2.imshow("Register Face", frame)
        if cv2.waitKey(1) & 0xFF == ord('q'):
            print("[INFO] 사용자가 종료하였습니다.")
            break

        time.sleep(delay)

    cap.release()
    cv2.destroyAllWindows()

    if len(embeddings) == 0:
        print("[ERROR] 얼굴을 등록하지 못했습니다.")
        return

    mean_embedding = np.mean(embeddings, axis=0)
    insert_user_embedding(user_info, mean_embedding)
    print(f"[INFO] {user_name} 얼굴 등록 완료, DB에 저장됨.")
    
    set_login_flag(user_info["name"], flag=1)
    print("[INFO] 로그인되었습니다.")

if __name__ == "__main__":
   user_info = { 
        "name" : input("등록할 이름을 입력하세요: ").strip(),
        "department" : input("부서를 입력해 주세요.").strip(),
        "position" : input("직책을 입력해 주세요.").strip()
    }
   
   if any(v == "" for v in user_info.values()):
       print("[ERROR] 모든 항목을 입력해 주세요.")
   else:
       capture_and_register(user_info, capture_time=10,delay=0.5)
