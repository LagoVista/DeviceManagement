# Device Access Manager Server

This project will provide services to handle requests from some sort of message queue technology, fulfil those requests and post the data where the client making the request can pick them up.

We will need to have some sort of security token, or maybe encrypt the data so that only the client making the request can get the data.

If possible this class should be 100% generic so we pass in a method that will actually fulfil the request and this class takes care of the plumbing.

The request should handle and closely model the REST methods such as GET, PUT, POST and DELETE

The app that will be hosting this service will likely be running on a private network, but will have access to the internet.