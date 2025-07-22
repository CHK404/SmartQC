import uuid
import mysql.connector
from mysql.connector import pooling

_POOL = pooling.MySQLConnectionPool(
    pool_name="smartqc_pool",
    pool_size=5,
    host='15.164.48.30',
    port=3306,
    user='dbchk',
    password='codingon2751',
    database='SmartQC',
    charset='utf8',
)

def get_connection():
    return _POOL.get_connection()

def get_logged_in_user() -> str:
    conn = get_connection()
    try:
        with conn.cursor() as cursor:
            cursor.execute(
                "SELECT UserName FROM UserData WHERE IsLoggedIn = 1 LIMIT 1"
            )
            row = cursor.fetchone()
            if row:
                return row[0]
    finally:
        conn.close()
    raise ValueError("로그인된 사용자가 없습니다.")

def log_error(serial_number: str, error_type: str) -> str:
    conn = get_connection()
    error_code = str(uuid.uuid4())
    try:
        with conn.cursor() as cursor:
            cursor.execute(
                "INSERT INTO Error (ErrorCode, ErrorType, SerialNumber) VALUES (%s, %s, %s)",
                (error_code, error_type, serial_number)
            )
        conn.commit()
    finally:
        conn.close()
    return error_code

def insert_product_detail(
    serial_number: str,
    user_name: str,
    product_name: str,
    is_defect: bool
):
    conn = get_connection()
    try:
        with conn.cursor() as cursor:
            cursor.execute(
                "INSERT INTO ProductDetail (SerialNumber, UserName, ProductName, Defection) VALUES (%s, %s, %s, %s)",
                (serial_number, user_name, product_name, int(is_defect))
            )
        conn.commit()
    finally:
        conn.close()

def upsert_product_data(
    product_name: str,
    product_info: str,
    delivery_due: str,
    required_quantity: int,
    is_defect: bool
):
    conn = get_connection()
    try:
        with conn.cursor() as cursor:
            cursor.execute(
                """
                INSERT INTO ProductData
                  (ProductName, Quantity, Defective, ProductInfo, DeliveryDueDate, RequiredQuantity)
                VALUES (%s, %s, %s, %s, %s, %s)
                ON DUPLICATE KEY UPDATE
                  Quantity  = Quantity  + 1,
                  Defective = Defective + VALUES(Defective)
                """,
                (product_name, 1, int(is_defect), product_info, delivery_due, required_quantity)
            )
        conn.commit()
    finally:
        conn.close()