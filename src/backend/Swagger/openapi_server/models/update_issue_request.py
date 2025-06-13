from openapi_server.models.base_model import Model

class UpdateIssueRequest:
    def __init__(self, description: str):
        self.description = description