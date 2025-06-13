from openapi_server.models.base_model import Model

class DeleteIssueRequest:
    def __init__(self, issue_id: int):
        self.issue_id = issue_id