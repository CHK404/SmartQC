from login import login_loop
from embedding_DB import capture_and_register

def main():
    while True:
        print("\n=== 얼굴 인식 시스템 ===")
        print("1. 얼굴 등록")
        print("2. 얼굴 로그인")
        print("3. 종료")
        choice = input("번호 선택: ").strip()
        
        if choice == '1':
            capture_and_register()
        elif choice == '2':
            username = login_loop()
            if username:
                return username
        elif choice == '3':
            print("프로그램 종료")
            break
        else:
            print("잘 못 입력하셨습니다.")
            
if __name__ == "__main__":
    main()