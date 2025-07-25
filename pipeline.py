import os
import sys

ROOT = os.path.dirname(__file__)
if ROOT not in sys.path:
    sys.path.insert(0, ROOT)

from Face_Login.main import main as face_main
from Defect_Detection.detect import DefectDetector
from Face_Login.embedding_DB import set_logout

def pipeline_main():
    user_name = face_main()
      
    try:
        detector = DefectDetector(user_name)
        detector.run()
    finally:
        set_logout(user_name)
        print(f"{user_name} 님 로그아웃 완료.")

    
if __name__ == "__main__":
    pipeline_main()
