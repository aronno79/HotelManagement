# Hotel API Documentation

**Base URL:** `http://localhost:5214`
**Swagger (test):** `http://localhost:5214/swagger`

## Rooms

| Action | Method | URL |
|---|---|---|
| Get all rooms | GET | `/api/Rooms` |
| Get one room | GET | `/api/Rooms/1` |
| Add room | POST | `/api/Rooms` |
| Edit room | PUT | `/api/Rooms/1` |
| Delete room | DELETE | `/api/Rooms/1` |

```json
{
  "roomNumber": "101",
  "roomType": "Single",
  "pricePerNight": 2500,
  "isAvailable": true
}
```

`roomType`: `Single`, `Double` or `Suite`. For edit, also send `"id": 1` in the body.

## Guests

| Action | Method | URL |
|---|---|---|
| Get all guests | GET | `/api/Guests` |
| Add guest | POST | `/api/Guests` |
| Delete guest | DELETE | `/api/Guests/1` |

```json
{
  "fullName": "Rahim Uddin",
  "phone": "01711111111",
  "email": "rahim@mail.com",
  "nidOrPassport": "1234567890"
}
```

`email` and `nidOrPassport` are optional.

## Bookings

| Action | Method | URL |
|---|---|---|
| Get all bookings | GET | `/api/Bookings` |
| Create booking | POST | `/api/Bookings` |
| Change status | PUT | `/api/Bookings/1/status?status=CheckedIn` |

```json
{
  "roomId": 1,
  "guestId": 1,
  "checkIn": "2026-11-10T00:00:00",
  "checkOut": "2026-11-13T00:00:00"
}
```

`totalPrice` is calculated by the server (nights x room price).
Status values: `Booked`, `CheckedIn`, `CheckedOut`, `Cancelled`.

## Rules

- Check-out must be after check-in
- Same room cannot be booked twice for overlapping dates (`Room is already booked for these dates.`)
- `CheckedIn` sets the room `isAvailable = false`
- `CheckedOut` or `Cancelled` sets it back to `true`

## Response codes

| Code | Meaning |
|---|---|
| 200 / 201 / 204 | Success |
| 400 | Bad data or rule broken |
| 404 | Not found |
| 500 | Server or database error |
