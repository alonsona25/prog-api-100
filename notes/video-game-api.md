# Video Game Sample API

Video games belong to a platform

- steam
- PC
- xbox
- ps5
- switch

```json
[
    {"id": 8398398, "name": "Xbox"},
    {"id": "3893893", "name": "Switch"}
]


```

## Add a platform to the API

```http
POST /platforms
Content-Type: application/json

{
    "name": "PS5"
}
```



## Get a list of platforms

## Add a game to the platform



---

GET /games

[
    {"id": "33", "platform": "Xbox", "name": "Balder's gate 3"}

]


GET /platforms/xbox/games



GET /v1/employees
Accept: application/json


GET /v2/pto-dates






## Versioning

- change the url (the resource)
- X - add a custom header
- X use content negotiation ("conneg")
- change the authority




https://sales.company.com/products

https://sales-2.company.com/products


versioning:

- can't remove things
- can't change the meaning of existing things (rarely tried, but gag)



GET /employees/x003809/performance-reviews

[

]



200 Ok
Content-Type: application/json

{
    "name": {
        "first": "Gretchen",
        "last": "Doe"
    },
    "contactInfo": {
        "phoneExtension": 39893839, 
        "email": "jane@aol.com"
    }
}


get /promotable-employees/{id}

[

    { id: 3939, name: 'bob smith', timeInPosition: 32, eligible: false}
]


POST /employees
Content-Type: application/json

{

}


POST /hiring-requests


201 
Location: /hiring-requests/398398

{
    "id":39839,
    "status": "Awaiting SSN"

}


```http
POST https://localhost:7268/platforms
Content-Type: application/json

{
    "name": "Switch"
}
```