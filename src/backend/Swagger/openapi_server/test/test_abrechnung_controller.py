import unittest
from unittest.mock import patch, MagicMock
from datetime import date

from openapi_server.controllers import abrechnung_controller

class TestAbrechnungController(unittest.TestCase):

    @patch('openapi_server.controllers.abrechnung_controller.psycopg2.connect')
    @patch('openapi_server.controllers.abrechnung_controller.jsonify')
    def test_get_abrechnungen_success(self, mock_jsonify, mock_connect):
        conn = MagicMock()
        cur = MagicMock()
        mock_connect.return_value = conn
        conn.cursor.return_value = cur

        row = {
            'aid': 100,
            'name': 'Testrechnung',
            'date': date(2024,5,5),
            'price': 250.75,
            'category': 'TestKat',
            'rechnung': 'file.pdf'
        }
        cur.fetchall.return_value = [row]
        mock_jsonify.return_value = "ok"

        result, code = abrechnung_controller.get_abrechnungen(7, 8)
        self.assertEqual(result, "ok")
        self.assertEqual(code, 200)
        mock_jsonify.assert_called_once()

    @patch('openapi_server.controllers.abrechnung_controller.psycopg2.connect')
    def test_get_abrechnungen_not_found(self, mock_connect):
        conn = MagicMock()
        cur = MagicMock()
        mock_connect.return_value = conn
        conn.cursor.return_value = cur
        cur.fetchall.return_value = []

        with self.assertRaises(Exception):
            abrechnung_controller.get_abrechnungen(1, 1)

    @patch('openapi_server.controllers.abrechnung_controller.psycopg2.connect')
    def test_get_abrechnungen_db_error(self, mock_connect):
        conn = MagicMock()
        cur = MagicMock()
        mock_connect.return_value = conn
        conn.cursor.return_value = cur
        cur.execute.side_effect = Exception("DB down")

        with self.assertRaises(Exception):
            abrechnung_controller.get_abrechnungen(1, 1)

    @patch('openapi_server.controllers.abrechnung_controller.psycopg2.connect')
    @patch('openapi_server.controllers.abrechnung_controller.jsonify')
    @patch('openapi_server.controllers.abrechnung_controller.request')
    def test_create_abrechnung_success(self, mock_request, mock_jsonify, mock_connect):
        payload = {
            "uid": 3,
            "pid": 4,
            "name": "Neue",
            "date": "2024-06-06",
            "price": 99.99,
            "category": "Cat",
            "rechnung": "r.pdf"
        }
        mock_request.get_json.return_value = payload

        conn = MagicMock()
        cur = MagicMock()
        conn.cursor.return_value.__enter__.return_value = cur
        mock_connect.return_value = conn

        returned = payload.copy()
        returned["aid"] = 55
        cur.fetchone.return_value = returned
        mock_jsonify.return_value = "created"

        result, code = abrechnung_controller.create_abrechnung()
        self.assertEqual(result, "created")
        self.assertEqual(code, 201)
        mock_jsonify.assert_called_once()

    @patch('openapi_server.controllers.abrechnung_controller.psycopg2.connect')
    @patch('openapi_server.controllers.abrechnung_controller.request')
    def test_create_abrechnung_missing_body(self, mock_request, mock_connect):
        mock_request.get_json.return_value = None
        with self.assertRaises(Exception):
            abrechnung_controller.create_abrechnung()

    @patch('openapi_server.controllers.abrechnung_controller.psycopg2.connect')
    @patch('openapi_server.controllers.abrechnung_controller.request')
    def test_create_abrechnung_missing_fields(self, mock_request, mock_connect):
        mock_request.get_json.return_value = {"uid": 1}
        with self.assertRaises(Exception):
            abrechnung_controller.create_abrechnung()

    @patch('openapi_server.controllers.abrechnung_controller.psycopg2.connect')
    @patch('openapi_server.controllers.abrechnung_controller.request')
    def test_create_abrechnung_db_error(self, mock_request, mock_connect):
        payload = {
            "uid": 3,
            "pid": 4,
            "name": "X",
            "date": "2024-06-06",
            "price": 1.23,
            "category": "C",
            "rechnung": "f"
        }
        mock_request.get_json.return_value = payload

        conn = MagicMock()
        cur = MagicMock()
        cur.execute.side_effect = Exception("DBERR")
        conn.cursor.return_value.__enter__.return_value = cur
        mock_connect.return_value = conn

        with self.assertRaises(Exception):
            abrechnung_controller.create_abrechnung()

if __name__ == '__main__':
    unittest.main()
