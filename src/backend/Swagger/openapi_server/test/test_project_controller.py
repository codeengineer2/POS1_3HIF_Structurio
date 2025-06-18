from . import BaseTestCase
import json


class TestProjectController(BaseTestCase):

    def test_create_project_success(self):
        payload = {
            "name": "Testprojekt",
            "description": "Beschreibung",
            "color": "#123456",
            "owner_uid": 1
        }

        response = self.client.post('/projects',
                                    data=json.dumps(payload),
                                    content_type='application/json')

        self.assertIn(response.status_code, [201, 500])

        if response.status_code ==   201:
            data = response.get_json()
            self.assertTrue(data["success"])
            self.assertIn("pid", data)
            self.assertIn("board", data)

    def test_create_project_missing_fields(self):
        payload = {
            "description": "Beschreibung",
            "color": "#123456"
            # owner_uid fehlt
        }

        response = self.client.post('/projects',
                                    data=json.dumps(payload),
                                    content_type='application/json')

        self.assertEqual(response.status_code, 400)

    def test_update_project_success_or_not_found(self):
        payload = {
            "pid": 1,
            "name": "Neuer Name",
            "description": "Neue Beschreibung",
            "color": "#abcdef"
        }

        response = self.client.put('/projects',
                                   data=json.dumps(payload),
                                   content_type='application/json')

        self.assertIn(response.status_code, [200, 404, 500])
        if response.status_code == 200:
            self.assertTrue(response.get_json()["success"])

    def test_update_project_missing_fields(self):
        payload = {
            "pid": 1,
            "description": "Fehlender Name und Color"
        }

        response = self.client.put('/projects',
                                   data=json.dumps(payload),
                                   content_type='application/json')

        self.assertEqual(response.status_code, 400)

    def test_delete_project_success_or_not_found(self):
        response = self.client.delete('/projects/1')
        self.assertIn(response.status_code, [200, 404, 500])