# GoWhere API - Frontend Integration Guide

This document describes existing API endpoints, request shapes and example responses to help frontend integration.

## Base URL
- Local (example): `https://localhost:{port}/api` or `http://localhost:{port}/api`

## Authentication
- JWT-based. Obtain a token from `POST /api/Auth/Login` (or `POST /api/Auth/LoginDemo` for demo user).
- Protected endpoints require header: `Authorization: Bearer {token}`.
- Swagger UI supports `Authorize` (enter `Bearer {token}`) once a token is obtained.

## Endpoints

### Auth
- `POST /api/Auth/Register`
  - Content-Type: `multipart/form-data` (controller expects `[FromForm]`).
  - Request model: `RegisterModel`:
    - `Username` (string, required)
    - `Password` (string, required)
    - `Email` (string, required)
    - `FullName` (string, required)
    - `PhoneNumber` (string, required)
    - `RoleId` (int, required)
  - Responses:
    - `200 OK` — success message
    - `400 Bad Request` — invalid data
    - `409 Conflict` — username exists

- `POST /api/Auth/Login`
  - Content-Type: `multipart/form-data` (controller expects `[FromForm]`).
  - Request model: `LoginModel`:
    - `Username` (string, required)
    - `Password` (string, required)
  - Response (200):
    ```json
    {
      "token": "<JWT token>",
      "message": "Login successfully."
    }
    ```
  - `401 Unauthorized` for bad credentials.

- `POST /api/Auth/LoginDemo` — returns token for demo user (`regularuser`) if present.

### ScenicSpot
- `GET /api/ScenicSpot/GetAllScenicSpots`
  - Returns list with fields: `Id`, `Name`, `Description`, `EstimatedVisitPrice`, `ProvinceId`, `Medias` (array of `{ MediaId, MediaUrl }`).

- `GET /api/ScenicSpot/GetScenicSpotById/{id}`
  - Returns single scenic spot with same shape as above or `404` if not found.

- `GET /api/ScenicSpot/GetScenicSpotsByProvinceId/{provinceId}`
  - Returns list filtered by `ProvinceId` or `404` if none.

- `PUT /api/ScenicSpot/AddScenicSpot`
  - Content-Type: `multipart/form-data`
  - Request model: `ScenicSpotCreateDto` (sent as form fields):
    - `ScenicSpotName` (string, required)
    - `Description` (string)
    - `EstimatedVisitPrice` (number)
    - `ProvinceId` (int)
    - `Medias` (one or more file inputs)
  - Behavior and validations:
    - Creates a `ScenicSpot` record first inside a DB transaction.
    - Saves files to disk under `wwwroot/uploads/scenicspots/{scenicSpotId}/`.
    - Allowed file extensions: `.jpg, .jpeg, .png, .gif, .bmp, .mp4, .mov, .avi, .mkv, .webm`.
    - Content type must start with `image/` or `video/`.
    - Total upload size limited to 50 MB per request.
    - Checks for suspicious file names (`..`, invalid chars) and available disk space before persisting.
    - Inserts `Media` records with `MediaUrl` (relative URL like `/uploads/scenicspots/{id}/{file}`).
  - Responses:
    - `201 Created` — returns location (`GetScenicSpotById`) and new `Id`.
    - `400 Bad Request` — validation failures or disallowed file types.
    - `507 Insufficient Storage` — disk space issues.
    - `500 Internal Server Error` — other failures.

  - Example curl (multipart):
    ```bash
    curl -X PUT "https://localhost:{port}/api/ScenicSpot/AddScenicSpot" \
      -H "Authorization: Bearer {token}" \
      -F "ScenicSpotName=My Spot" \
      -F "Description=Nice place" \
      -F "EstimatedVisitPrice=12.5" \
      -F "ProvinceId=1" \
      -F "Medias=@/path/to/photo1.jpg" \
      -F "Medias=@/path/to/video1.mp4"
    ```

### Misc
- `GET /WeatherForecast` — sample endpoint returning weather data.

## Models (summary)
- `ScenicSpot` (server model): `ScenicSpotId`, `ScenicSpotName`, `Description`, `EstimatedVisitPrice`, `ProvinceId`, `Medias` (collection)
- `Media`: `MediaId`, `MediaUrl`, `ScenicSpotId`
- `Province`: `ProvinceId`, `ProvinceName`

## Notes & Recommendations for Frontend
- All protected API calls must include the `Authorization` header with the JWT token.
- Upload files via `multipart/form-data` using form fields named exactly as the DTO properties (e.g., `Medias` for multiple files).
- After creating a scenic spot, uploaded files will be served from `https://{host}/uploads/scenicspots/{id}/{filename}` (static files from `wwwroot`).
- If Swagger UI is enabled in development, use `POST /api/Auth/Login` to get a token then press `Authorize` to paste `Bearer {token}`.
- Backend validates file names, types and disk space. Handle `507` responses as storage errors.

If you want, a JSON OpenAPI spec or example Postman collection can be exported next.