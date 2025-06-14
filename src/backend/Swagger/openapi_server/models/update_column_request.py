from openapi_server.models.base_model import Model

class UpdateColumnRequest(Model):
    def __init__(self, id=None, name=None):
        self.openapi_types = {'id': int, 'name': str}
        self.attribute_map = {'id': 'id', 'name': 'name'}

        self.id = id
        self.name = name