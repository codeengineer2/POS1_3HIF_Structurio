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
        SELECT uid, firstname, lastname, email, birthdate
        FROM users
        WHERE email = %s AND password_hash = %s
    """, (email, password))
    user_row = cur.fetchone()

    if not user_row:
        cur.close()
        conn.close()
        return {
            "success": False,
            "code": "LOGIN_FAILED",
            "message": "E-Mail oder Passwort ungültig"
        }, 401

    uid, firstname, lastname, email, birthdate = user_row

    cur.execute("""
        SELECT pid, name, description, color FROM projects
        WHERE owner_uid = %s
    """, (uid,))
    
    projects = []
    
    for pid, name, description, color in cur.fetchall():
        cur.execute("SELECT bid FROM boards WHERE pid = %s", (pid,))
        board_row = cur.fetchone()
        board_id = board_row[0] if board_row else None
        board_obj = {"id": board_id, "project_id": pid, "columns": []}

        cur.execute("SELECT cid, name FROM columns WHERE bid = %s", (board_id,))
        
        for cid, cname in cur.fetchall():
            cur.execute("SELECT iid, description FROM issues WHERE cid = %s", (cid,))
            issues = [{"id": iid, "description": desc, "column_id": cid} for iid, desc in cur.fetchall()]
            board_obj["columns"].append({
                "id": cid, "name": cname, "board_id": board_id, "issues": issues
            })

        projects.append({
            "pid": pid,
            "name": name,
            "description": description,
            "color": color,
            "owner_uid": uid,
            "board": board_obj
        })

    cur.close()
    conn.close()

    return {
        "success": True,
        "user": {
            "uid": uid,
            "firstname": firstname,
            "lastname": lastname,
            "email": email,
            "birthdate": str(birthdate)
        },
        "projects": projects
    }, 200

def auth_register_post(body):
    firstname = body.get("firstname") if isinstance(body, dict) else body.firstname
    lastname = body.get("lastname") if isinstance(body, dict) else body.lastname
    email = body.get("email") if isinstance(body, dict) else body.email
    password = body.get("password") if isinstance(body, dict) else body.password
    birthdate = body.get("birthdate") if isinstance(body, dict) else body.birthdate

    conn = get_connection()
    cur = conn.cursor()
    
    try:
        cur.execute(
            """
            INSERT INTO users (firstname, lastname, email, password_hash, birthdate)
            VALUES (%s, %s, %s, %s, %s)
            RETURNING uid
            """,
            (firstname, lastname, email, password, birthdate)
        )
        uid = cur.fetchone()[0]
        conn.commit()

        return {
            "success": True,
            "user": {
                "uid": uid,
                "firstname": firstname,
                "lastname": lastname,
                "email": email,
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