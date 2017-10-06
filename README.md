# RatAppRESTful
A RESTful API for the CS2340 Rat App (part of the Muffin Dreamers group). Deploys as an Azure App Service

## User
### Registration
POST Request with JSON object in body containing username, password, and optionally role.

Endpoint: \<host\>/user/register

Response Codes: 400 Bad Request, 409 Conflict, or 201 Created

Sample JSON object:
```json
{
  "username":"testuser@email.com",
  "password":"mypassword123",
  "role":"User"
}
```

### Authentication
POST Request with JSON object in body containing username and password. If successfully authenticated, returns a base64 access token.

Endpoint: \<host\>/user/auth

Resonse Codes: 400 Bad Request, 401 Unathorized, 200 Ok

### Logout
GET Request with token parameter in URL. Requires a valid access token. This will revoke the specified token.

Endpoint: \<host\>/user/logout?token=\<access token\>

Response Codes: 401 Unathorized, 200 Ok

### User Info
GET Request that looks up a user by ID and returns a JSON object representing a User. NOTE password will always be left blank. Requires a valid access token.

Endpoint: \<host\>/user/\<user id\>?token=\<access token\>

Response Codes: 401 Unathorized, 404 Not Found, 200 Ok

Sample JSON object in response
```JSON
{
  "id":7,
  "username":"testuser@email.com",
  "password":"",
  "role":"Admin"
}
```

### Delete User
DELETE Request that deletes a user specified by ID. Requires a valid access token associated with an account with the Admin role

Endpoint: \<host\>/user/delete/\<user id\>?token=\<access token\>

Response Codes: 401 Unathorized, 404 Not Found, 200 Ok

## Sighting

### Get All Sightings
GET Request that returns a JSON array of all sighting objects in the database

Endpoint: \<host\>/sightings

Response Codes: 200 Ok

Sample JSON array:
```JSON
[
    {
        "id": 12,
        "dateCreated": "2017-09-05T11:38:39",
        "locationType": "type2",
        "zipcode": 65423,
        "streetAddress": "123 Street Dr.",
        "city": "New York",
        "borough": "Test Borough 3",
        "latitude": 40.71448,
        "longitude": -74.00598
    },
    {
        "id": 13,
        "dateCreated": "2017-09-05T16:11:01",
        "locationType": "type1",
        "zipcode": 65423,
        "streetAddress": "456 A St.",
        "city": "New York",
        "borough": "Test Borough 3",
        "latitude": 40.71199,
        "longitude": -74.00999
    },
    {
        "id": 14,
        "dateCreated": "2017-10-05T08:49:07",
        "locationType": "type1",
        "zipcode": 65423,
        "streetAddress": "9874 19th St.",
        "city": "New York",
        "borough": "Test Borough 2",
        "latitude": 40.72985,
        "longitude": -74.02154
    }
]
```

### Get Sighting
GET Request that returns a JSON object for the specified sighting ID

Endpoint: \<host\>/sightings/\<sighting id\>

Response Codes: 404 Not Found, 200 Ok

Sample JSON object:
```JSON
{
    "id": 12,
    "dateCreated": "2017-09-05T11:38:39",
    "locationType": "type2",
    "zipcode": 65423,
    "streetAddress": "123 Street Dr.",
    "city": "New York",
    "borough": "Test Borough 3",
    "latitude": 40.71448,
    "longitude": -74.00598
}
```

### New Report
POST Request with a JSON object in the body representing the report. Must have a valid access token. id property will be ignored and a new id automatically generated. Response will contain a copy of the JSON object that is on the server, with the new id.

Endpoint: \<host\>/sightings/report?token=\<access token\>

Response Codes: 401 Unathorized, 400 Bad Request, 201 Created

See above for sample JSON objects

### Delete Sighting

DELETE Request that deletes the report with the specified id. Must have a valid access token associated with an account with the Admin role.

Endpoint: \<host\>/sightings/delete/\<id\>?token=\<access token\>

Response Codes: 401 Unathorized, 404 Not Found, 200 Ok
