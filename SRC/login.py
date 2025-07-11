import cv2
import numpy as np
from insightface.app import FaceAnalysis
from numpy.linalg import norm
import mysql.connector
import time
from PIL import ImageFont, ImageDraw, Image

db_config = {
    'host': '15.164.48.30',
    'port': 3306,
    'user': 'dbchk',
    'password': 'codingon2751',
    'database': 'SmartQC'
}

app = FaceAnalysis(providers=['CPUExecutionProvider'])
app.prepare(ctx_id=0, det_size=(640, 640))

SIMILARITY_THRESHOLD = 0.6

def l2_normalize(x):
    return x / norm(x)

def cosine_similarity(a, b):
    return np.dot(a, b)

def load_registered_users():
    conn = mysql.connector.connect(**db_config)
    cursor = conn.cursor()
    cursor.execute("SELECT UserName, Department, Position, FaceID, IsLoggedIn FROM UserData")
    users = []
    for username, department, position, faceid_bytes, isLoggedIn in cursor.fetchall():
        if faceid_bytes is None:
            print(f"[WARNING] 사용자 '{username}' 의 FaceID가 없습니다. 건너뜁니다.")
            continue
        embedding = np.frombuffer(faceid_bytes, dtype=np.float32)
        users.append({
            'name': username,
            'department': department,
            'position': position,
            'embedding': embedding,
            'isLoggedIn' : isLoggedIn
        })
    cursor.close()
    conn.close()
    return users

def put_text_kr(img, text, pos, font_path='C:/Windows/Fonts/malgun.ttf', font_size=30, color=(255,255,255)):
    img_pil = Image.fromarray(cv2.cvtColor(img, cv2.COLOR_BGR2RGB))
    draw = ImageDraw.Draw(img_pil)
    font = ImageFont.truetype(font_path, font_size)
    draw.text(pos, text, font=font, fill=color[::-1])
    img = cv2.cvtColor(np.array(img_pil), cv2.COLOR_RGB2BGR)
    return img

def select_user(users):
    print("등록된 사용자 목록:")
    for i, user in enumerate(users):
        print(f"{i+1}. {user['name']} / {user['department']} / {user['position']}")
    while True:
        try:
            choice = int(input("로그인할 사용자 번호를 선택하세요: "))
            if 1 <= choice <= len(users):
                return users[choice - 1]
            else:
                print("올바른 번호를 입력하세요.")
        except ValueError:
            print("숫자를 입력하세요.")

def login_loop():
    users = load_registered_users()
    if len(users) == 0:
        print("[ERROR] 등록된 사용자가 없습니다.")
        return

    selected_user = select_user(users)
    print(f"[INFO] 선택된 사용자: {selected_user['name']}")

    cap = cv2.VideoCapture(1)
    if not cap.isOpened():
        print("[ERROR] 카메라를 열지 않았습니다.")
        return

    print("[INFO] 얼굴 인식 로그인 시작. 종료하려면 'q' 키를 누르세요.")

    while True:
        ret, frame = cap.read()
        if not ret:
            continue

        faces = app.get(frame)
        if len(faces) > 0:
            face_emb = l2_normalize(faces[0].embedding)
            similarity = cosine_similarity(face_emb, selected_user['embedding'])
            if similarity > SIMILARITY_THRESHOLD:
                text = f"{selected_user['name']} 로그인 성공! ({similarity:.2f})"
                color = (0, 255, 0)
                frame = put_text_kr(frame, text, (20, 50), font_size=28, color=color)
                cv2.imshow("Face Login", frame)
                cv2.waitKey(2000) 
                print(f"[INFO] {selected_user['name']} 로그인 성공, 프로그램 종료합니다.")
                break
            else:
                text = f"얼굴 불일치 ({similarity:.2f})"
                color = (0, 0, 255)
        else:
            text = "얼굴 미검출"
            color = (0, 0, 255)

        frame = put_text_kr(frame, text, (20, 50), font_size=24, color=color)
        cv2.imshow("Face Login", frame)

        if cv2.waitKey(1) & 0xFF == ord('q'):
            print("[INFO] 프로그램 종료")
            break

    cap.release()
    cv2.destroyAllWindows()

if __name__ == "__main__":
    login_loop()
