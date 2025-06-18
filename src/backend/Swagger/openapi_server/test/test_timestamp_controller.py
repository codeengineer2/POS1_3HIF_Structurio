import unittest
from unittest.mock import patch, MagicMock
from datetime import date, time

# Den Flask-App-Objekt importierst du aus app.py, nicht aus __init__.py
from openapi_server import app
from openapi_server.controllers import timestamp_controller

class TestTimestampController(unittest.TestCase):

    def setUp(self):
        # Application- und Request-Context für jsonify() & abort()
        self.ctx = app.test_request_context()
        self.ctx.push()

    def tearDown(self):
        self.ctx.pop()

    @patch('openapi_server.controllers.timestamp_controller.get_connection')
    def test_get_timestamps_by_user_success(self, mock_conn):
        # Simuliere einen Datensatz mit Strings direkt (ISO-Format)
        row = {
            "zid": 1,
            "datum_in": "2024-01-01",
            "checkin": "08:00:00",
            "datum_out": "2024-01-01",
            "checkout": "16:00:00",
            "duration": "08:00"
        }
        mock_cursor = MagicMock()
        mock_cursor.fetchall.return_value = [row]
        mock_conn.return_value.cursor.return_value = mock_cursor

        response, status = timestamp_controller.get_timestamps_by_user(42)
        self.assertEqual(status, 200)
        data = response.get_json()
        self.assertEqual(len(data), 1)
        self.assertEqual(data[0]["zid"], 1)
        self.assertEqual(data[0]["datum_in"], "2024-01-01")
        self.assertEqual(data[0]["checkin"], "08:00:00")

    @patch('openapi_server.controllers.timestamp_controller.get_connection')
    def test_get_timestamps_by_user_not_found(self, mock_conn):
        mock_cursor = MagicMock()
        mock_cursor.fetchall.return_value = []
        mock_conn.return_value.cursor.return_value = mock_cursor

        with self.assertRaises(Exception):
            timestamp_controller.get_timestamps_by_user(99)

    @patch('openapi_server.controllers.timestamp_controller.get_connection')
    def test_get_timestamps_by_user_db_error(self, mock_conn):
        mock_cursor = MagicMock()
        mock_cursor.execute.side_effect = Exception("DB-Fehler")
        mock_conn.return_value.cursor.return_value = mock_cursor

        with self.assertRaises(Exception):
            timestamp_controller.get_timestamps_by_user(1)

    @patch('openapi_server.controllers.timestamp_controller.get_connection')
    def test_get_timestamp_by_id_success(self, mock_conn):
        record = {
            "zid": 5,
            "uid": 2,
            "checkin": "07:30:00",
            "checkout": "15:30:00",
            "datum_in": "2024-03-03",
            "datum_out": "2024-03-03",
            "duration": "08:00"
        }
        mock_cursor = MagicMock()
        mock_cursor.fetchone.return_value = record
        mock_conn.return_value.cursor.return_value = mock_cursor

        response, status = timestamp_controller.get_timestamp_by_id(5)
        self.assertEqual(status, 200)
        data = response.get_json()
        self.assertEqual(data["zid"], 5)
        self.assertEqual(data["datum_in"], "2024-03-03")

    @patch('openapi_server.controllers.timestamp_controller.get_connection')
    def test_get_timestamp_by_id_not_found(self, mock_conn):
        mock_cursor = MagicMock()
        mock_cursor.fetchone.return_value = None
        mock_conn.return_value.cursor.return_value = mock_cursor

        with self.assertRaises(Exception):
            timestamp_controller.get_timestamp_by_id(123)

    @patch('openapi_server.controllers.timestamp_controller.request')
    @patch('openapi_server.controllers.timestamp_controller.get_connection')
    def test_create_timestamp_success(self, mock_conn, mock_request):
        payload = {
            "uid": 7,
            "datum_in": "2024-02-02",
            "datum_out": "2024-02-02",
            "checkin": "09:00:00",
            "checkout": "17:00:00",
            "duration": "08:00"
        }
        mock_request.get_json.return_value = payload
        returned = payload.copy()
        returned["zid"] = 99

        mock_cursor = MagicMock()
        mock_cursor.fetchone.return_value = returned
        mock_conn.return_value.cursor.return_value = mock_cursor

        response, status = timestamp_controller.create_timestamp()
        self.assertEqual(status, 201)
        data = response.get_json()
        self.assertEqual(data["zid"], 99)
        self.assertEqual(data["uid"], 7)

    @patch('openapi_server.controllers.timestamp_controller.request')
    @patch('openapi_server.controllers.timestamp_controller.abort')
    def test_create_timestamp_missing_fields(self, mock_abort, mock_request):
        mock_request.get_json.return_value = {"checkin": "x"}
        timestamp_controller.create_timestamp()
        mock_abort.assert_called_with(400, description="'uid' ist erforderlich")

    @patch('openapi_server.controllers.timestamp_controller.request')
    @patch('openapi_server.controllers.timestamp_controller.get_connection')
    def test_update_timestamp_success(self, mock_conn, mock_request):
        payload = {
            "datum_in": "2024-04-04",
            "checkin": "08:00:00",
            "datum_out": "2024-04-04",
            "checkout": "16:00:00",
            "duration": "08:00"
        }
        mock_request.get_json.return_value = payload
        returned = payload.copy()
        returned.update({"zid": 10, "uid": 3})

        mock_cursor = MagicMock()
        mock_cursor.fetchone.return_value = returned
        mock_conn.return_value.cursor.return_value = mock_cursor

        response, status = timestamp_controller.update_timestamp(3, 10)
        self.assertEqual(status, 200)
        data = response.get_json()
        self.assertEqual(data["zid"], 10)

    @patch('openapi_server.controllers.timestamp_controller.request')
    @patch('openapi_server.controllers.timestamp_controller.abort')
    def test_update_timestamp_missing_fields(self, mock_abort, mock_request):
        mock_request.get_json.return_value = {"checkin": "x"}
        timestamp_controller.update_timestamp(1, 1)
        mock_abort.assert_called_with(400, "'datum_in' ist erforderlich")

if __name__ == '__main__':
    unittest.main()
