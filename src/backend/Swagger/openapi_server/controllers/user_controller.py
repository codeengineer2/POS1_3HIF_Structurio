import psycopg2
from openapi_server.models.login_request import LoginRequest
from openapi_server.models.email_request import EmailRequest

def get_connection():
    # egal weil egal
    conn_str = (
        "postgresql://structure_owner:npg_cEPXthQ49IRm@"
        "ep-calm-grass-a272ihxj-pooler.eu-central-1.aws.neon.tech/"
        "structure?sslmode=require"
    )
    return psycopg2.connect(conn_str)

def auth_check_email_post(body):
    email = body.get("email") if isinstance(body, dict) else body.email

    conn = get_connection()
    cur = conn.cursor()
    cur.execute("SELECT 1 FROM users WHERE email = %s", (email,))
    found = cur.fetchone()
    cur.close()
    conn.close()

    if found:
        return {"exists": True}, 200
    else:
        return {"error": "E-Mail nicht gefunden"}, 404

def auth_login_post(body):
    email = body.get("email") if isinstance(body, dict) else body.email
    password = body.get("password") if isinstance(body, dict) else body.password

    conn = get_connection()
    cur = conn.cursor()

    cur.execute("""
        SELECT uid, birthdate, password
        FROM users
        WHERE email = %s AND password = %s
    """, (email, password))
    user = cur.fetchone()

    if not user:
        cur.close()
        conn.close()
        return {
            "success": False,
            "code": "LOGIN_FAILED",
            "message": "E-Mail oder Passwort ungültig"
        }, 401

    uid, birthdate, stored_password = user
    cur.close()
    conn.close()

    return {
        "success": True,
        "user": {
            "uid": uid,
            "email": email,
            "password": stored_password,
            "birthdate": str(birthdate)
        }
    }, 200

def auth_register_post(body):
    surname = body.get("surname") if isinstance(body, dict) else body.surname
    lastname = body.get("lastname") if isinstance(body, dict) else body.lastname
    email = body.get("email") if isinstance(body, dict) else body.email
    password = body.get("password") if isinstance(body, dict) else body.password
    birthdate = body.get("birthdate") if isinstance(body, dict) else body.birthdate

    conn = get_connection()
    cur = conn.cursor()
    try:
        cur.execute(
            """
            INSERT INTO users (surname, lastname, email, password, birthdate)
            VALUES (%s, %s, %s, %s, %s)
            RETURNING uid
            """,
            (surname, lastname, email, password, birthdate)
        )
        uid = cur.fetchone()[0]
        conn.commit()

        return {
            "success": True,
            "user": {
                "uid": uid,
                "surname": surname,
                "lastname": lastname,
                "email": email,
                "password": password,
                "birthdate": str(birthdate)
            }
        }, 201

    except psycopg2.errors.UniqueViolation:
        conn.rollback()
        return {
            "success": False,
            "code": "EMAIL_EXISTS",
            "message": "Diese E-Mail ist bereits registriert"
        }, 409

    finally:
        cur.close()
        conn.close()