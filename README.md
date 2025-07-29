# SmartQC - Project
얼굴 인식 기반 품질 관리 시스템
## 💻 프로젝트 소개
> **얼굴 인식을 기반으로 한 스마트 팩토리 품질 관리 솔루션**
> - ✅ 출퇴근 자동화
> - ✅ 불량품 실시간 감자
> - ✅ MySQL 기반 데이터 기록 및 분석

## 🕰️ 개발 기간
> 25.07.01 ~ 25.07.28
---

## 멤버구성
> 팀장 ( 김철희) : DB 설계 및 연동, C# WPF <br>
> 팀원 ( 박현승 ) : UI 제작, 얼굴 인식 로그인 <br>
> 팀원 ( 서창범 ) : PLC 연동, OpenCV 활용 불량품 검출

## 🗒️ 목차
- [ 🔴 프로젝트 소개](#-프로젝트-소개)
- [ 🔧 기술 스택](#-기술-스택)
- [📷 스크린샷](#-스크린샷)
- [⚙️ 시스템 구조](#-시스템-구조)

  ---

  ## 🔴 프로젝트 소개
  **“SMART QC”란 OPEN CV를 활용한 “얼굴 인식 로그인 시스템”과
  PLC를 활용한 “불량 검출 시스템”을 위하여 만들어진 ‘미니 공정 설계’ 입니다.**

  ---

  ## 🔧 기술 스택
  ![Python]
  ![C#]
  ![MySQL]
  ![OpenCV]

  - **Python** - 얼굴 인식 (OpenCV, InsightFace)
  - **C# WPF** - UI & 관리자 시스템
  - **MySQL** - 데이터 저장 및 관리
  - **YOLO** - 불량품 탐지

  ---

  ## 📷 스크린샷
  - **얼굴 인식 로그인**
  >  <img width="802" height="602" alt="Frame 1" src="https://github.com/user-attachments/assets/40a1c57b-09ec-4cdf-baec-417c826ceaf0" />

- **불량품 감지**
  > <img width="606" height="324" alt="image 8" src="https://github.com/user-attachments/assets/71dd8014-be1f-4ab9-b54c-782ba4a04d40" />
  > <img width="606" height="442" alt="image 9" src="https://github.com/user-attachments/assets/961a2e3f-2827-4147-b2ee-9a618107a4a9" />


---


  ## ⚙️ 시스템 구조
  > ## 🔁 데이터 흐름 <br>
  > 1️⃣ Python **카메라 영상 -> 얼굴 임베딩 -> DB와 비교** <br>
  > 2️⃣ MySQL DB 결과 반환 -> WPF 앱에 로그인 상태 업데이트 <br>
  > 3️⃣ 로그인 성공시 -> 권한별 (관리자 / 사원) 화면 및 기능 제공 <br>
  > 4️⃣ 불량품 감지 시 -> PLC 전송 & DB 기록

---

## ❗ 트러블 슈팅
- 
