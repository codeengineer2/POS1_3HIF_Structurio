from openapi_server.models.base_model import Model

class UpdateProjectRequest(Model):
    def __init__(self, pid=None, name=None, description=None, color=None):
        self.openapi_types = {
            'pid': int,
            'name': str,
            'description': str,
            'color': str
        }

        self.attribute_map = {
            'pid': 'pid',
            'name': 'name',
            'description': 'description',
            'color': 'color'
        }

        self.pid = pid
        self.name = name
        self.description = description
        self.color = color