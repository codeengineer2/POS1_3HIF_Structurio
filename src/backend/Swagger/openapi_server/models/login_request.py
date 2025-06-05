from openapi_server.models.base_model import Model

class LoginRequest(Model):
    def __init__(self, email=None, password=None):
        self.openapi_types = {
            'email': str,
            'password': str
        }
        self.attribute_map = {
            'email': 'email',
            'password': 'password'
        }

        self.email = email
        self.password = password