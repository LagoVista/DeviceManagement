# Data Access Manager Client

The project will act as a proxy to request data from a private network.

Essentially it will:
1) Drop a request (that may include a payload) into some sort of message queue technology, that request will also need some sort of token used for security.
2) Block while the request is being processed and monitor where the result will be published
3) Have a configurable timeout, if the timeout occurs, the response will return an InvokeResult with an error timeout message
4) Once it sees the response has been published it will return it to the calling method.

Note: To the consumer of this service, it will just act as an async request, it might even act as a repo