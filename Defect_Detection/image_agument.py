import cv2 #OpenCV 이미지 처리용 라이브러리
import os #운영 체제 관련 작업(폴더 만들기, 파일 목록 가져오기 등)
#PIL: 이미지 열기/회전/밝기 조정 등을 위한 라이브러리
#Image: 각도/회전/경로로 열기/저장 등등의 해주는 도구
#ImageEnhance: 밝기,대비,색조,선명도 등을 조절해주는 도구
from PIL import Image, ImageEnhance 


input_dir = r"D:\cap_dataset2" #원본 이미지가 들어있는 폴더 경로
output_dir = r"D:\cap_dataset_aug" #증강된 이미지를 저장할 새 폴더 경로

os.makedirs(output_dir,exist_ok=True)
#output_dir 경로에 폴더를 만듦
#이미 만들어진 폴더가 있다면 에러 없이 넘어감 ---> exist_ok=True

for filename in os.listdir(input_dir):    
#input_dir 안에 있는 파일/폴더 '이름'들을 리스트로 반환
#리스트 안에서 항목을 하나씩 filename 변수에 담아 반복 실행
    if not filename.endswith(".jpg"):
        continue
    #.jpg 아니면 무시하고 다음 파일로 넘어가라
    
    path = os.path.join(input_dir,filename)
    #전체 파일 경로를 만듦 --> D:\cap_dataset\cap_ok_1.jpg
    #filename은 이름만 담고 있기 때문에 다시 경로를 설정해야함 
    img = Image.open(path)
    img = img.resize((640, 640))
    #이미지 파일을 열어서 변수 img에 저장
    for i in range(5):
        rotated = img.rotate((i-3)*45)
        #-90,-45,0,45,90도로 이미지 회전
        #i=0 ---> -90도 회전 i=1 ---> -45도 회전
        enhancer = ImageEnhance.Brightness(rotated)
        bright = enhancer.enhance(1 + 0.2*(i-3))
        
        new_name = filename.replace(".jpg",f"_aug{i+1}.jpg")
        bright.save(os.path.join(output_dir, new_name))
import cv2
import os
from PIL import Image, ImageEnhance

input_dir = r"D:\cap_dataset2"         # 원본 이미지 폴더
output_dir = r"D:\cap_dataset_aug"    # 증강 이미지 저장 폴더
os.makedirs(output_dir, exist_ok=True)

for filename in os.listdir(input_dir):
    if not filename.endswith(".jpg"):
        continue
    
    path = os.path.join(input_dir, filename)
    img = Image.open(path).convert("RGB")
    img = img.resize((640, 640))  # 기본 크기 맞춤

    for i in range(5):  # 회전 + 밝기 조절
        rotated = img.rotate((i - 2) * 45)  # -90 ~ +90도
        enhancer = ImageEnhance.Brightness(rotated)
        bright = enhancer.enhance(1 + 0.2 * (i - 2))  # 밝기 ±

        for scale_idx, scale in enumerate([0.5, 0.8, 1.0, 1.2, 1.5]):  # 거리 다양화
            # 거리(스케일)에 따라 확대/축소
            w, h = bright.size
            new_w = int(w * scale)
            new_h = int(h * scale)
            resized = bright.resize((new_w, new_h))

            # 640x640 배경에 중앙 배치
            background = Image.new("RGB", (640, 640), (0, 0, 0))
            paste_x = (640 - new_w) // 2
            paste_y = (640 - new_h) // 2
            background.paste(resized, (paste_x, paste_y))

            # 파일 저장
            new_name = filename.replace(".jpg", f"_aug{i+1}_scale{scale_idx+1}.jpg")
            background.save(os.path.join(output_dir, new_name))