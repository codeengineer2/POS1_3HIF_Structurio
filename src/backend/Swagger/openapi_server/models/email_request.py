from openapi_server.models.base_model import Model

class EmailRequest(Model):
    def __init__(self, email=None):
        self.openapi_types = {
            'email': str
        }
        self.attribute_map = {
            'email': 'email'
        }
        self.email = email