import psycopg2
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