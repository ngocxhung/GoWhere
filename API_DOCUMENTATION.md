# GoWhere API - Frontend Integration Guide

This document describes the available API endpoints, request/response models and examples to help frontend developers integrate with the GoWhere backend.

Base URL (local)
- Example: `https://localhost:{port}/api` or `http://localhost:{port}/api`

Authentication
- JWT-based. Obtain a token from `POST /api/Auth/Login` (or `POST /api/Auth/LoginDemo` for demo user).
- Protected endpoints require the HTTP header: `Authorization: Bearer {token}`.

Swagger / testing
- Swagger UI is available in Development at `/swagger/index.html` when enabled. If the Authorize dialog is present you can paste `Bearer {token}` to test protected endpoints. Otherwise add the header manually in your test client.

Endpoints

1) Auth
- `POST /api/Auth/Register`
  - Content-Type: `multipart/form-data` (controller expects `[FromForm]`).
  - Request model: `RegisterModel` (form fields):
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
  - Request model: `LoginModel` (form fields):
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

- `POST /api/Auth/LoginDemo` — returns a token for the seeded demo user (`regularuser`) if present. Use for quick testing.

2) Province
- `GET /api/Province/GetAllProvinces` — returns list of provinces (id/name).
- `GET /api/Province/GetProviceById/{id}` — returns province by id or `404` if not found.
  - Note: route name contains a small typo in controller (`GetProviceById`) — use the exact route above when calling.

3) ScenicSpot
- `GET /api/ScenicSpot/GetAllScenicSpots`
  - Response: array of scenic spots. Each item contains:
    - `Id` (int)
    - `Name` (string)
    - `Description` (string)
    - `EstimatedVisitPrice` (number)
    - `ProvinceId` (int)
    - `Medias` (array of `{ MediaId, MediaUrl }`)

- `GET /api/ScenicSpot/GetScenicSpotById/{id}` — returns a single scenic spot or `404`.

- `GET /api/ScenicSpot/GetScenicSpotsByProvinceId/{provinceId}` — filtered list by province.

- `PUT /api/ScenicSpot/AddScenicSpot`
  - Content-Type: `multipart/form-data`
  - Request model (form fields): `ScenicSpotRequestDto`:
    - `ScenicSpotName` (string, required)
    - `Description` (string, required)
    - `EstimatedVisitPrice` (number, required)
    - `ProvinceId` (int)
    - `Medias` (one or more files, required)
  - Behavior & validations (server-side):
    - A minimal `ScenicSpot` record is created first, files are uploaded to `wwwroot/uploads/scenicspots/{scenicSpotId}/` and `Media` rows are added.
    - Allowed extensions: `.jpg, .jpeg, .png, .gif, .bmp, .mp4, .mov, .avi, .mkv, .webm`.
    - Content type must start with `image/` or `video/`.
    - Maximum total upload size: 250 MB (service-level limit).
    - Filenames are validated against path traversal and invalid chars.
    - Disk space is checked before saving each file. On any failure uploaded files are deleted and the operation is rolled back.
  - Responses:
    - `201 Created` — created successfully (returns location route to GetScenicSpotById).
    - `400 Bad Request` — validation failure (missing fields / bad files).
    - `507 Insufficient Storage` — disk space issue.
    - `500 Internal Server Error` — unexpected errors.

  - Example (curl):
    ```bash
    curl -X PUT "https://localhost:{port}/api/ScenicSpot/AddScenicSpot" \
      -H "Authorization: Bearer {token}" \
      -F "ScenicSpotName=My Spot" \
      -F "Description=Nice place" \
      -F "EstimatedVisitPrice=12.5" \
      -F "ProvinceId=1" \
      -F "Medias=@/path/to/photo1.jpg" \
      -F "Medias=@/path/to/photo2.png"
    ```

- `PUT /api/ScenicSpot/UpdateScenicSpot/{id}`
  - Content-Type: `multipart/form-data` (use form fields and file fields)
  - Request model: `ScenicSpotUpdateRequestDto` supports two media-related inputs:
    - `ExistingMedias` (collection of strings) — relative URLs of media to keep (e.g. `/uploads/scenicspots/12/file.jpg`). Any media currently associated with the scenic spot that is NOT included in `ExistingMedias` will be removed (DB row deleted and file deleted from disk).
    - `NewMedias` (one or more files) — files to upload and attach to the scenic spot.
    - Other fields: `ScenicSpotName`, `Description`, `EstimatedVisitPrice`, `ProvinceId`.
  - Update behavior:
    - Server validates input; uploads new files using the same rules as `AddScenicSpot` (extensions, content type, disk space).
    - Files listed in `ExistingMedias` are preserved.
    - Files previously present but not included in `ExistingMedias` will be deleted from disk and removed from DB.
    - The update is performed in a transactional manner where DB updates and file operations are coordinated; on failure uploaded new files are cleaned up.
  - Responses:
    - `200 OK` — update successful
    - `400 Bad Request` — validation errors
    - `404 Not Found` — scenic spot not found
    - `507 Insufficient Storage` — disk space issues
    - `500 Internal Server Error` — unexpected errors

  - Example (curl): keep one existing media and add a new file:
    ```bash
    curl -X PUT "https://localhost:{port}/api/ScenicSpot/UpdateScenicSpot/12" \
      -H "Authorization: Bearer {token}" \
      -F "ScenicSpotName=Updated Name" \
      -F "Description=Updated description" \
      -F "EstimatedVisitPrice=20" \
      -F "ProvinceId=1" \
      -F "ExistingMedias=/uploads/scenicspots/12/keep.jpg" \
      -F "NewMedias=@/path/to/newphoto.jpg"
    ```

4) Files / Media
- Files are stored under `wwwroot/uploads/scenicspots/{scenicSpotId}/` and served statically at `/uploads/scenicspots/{id}/{filename}`.
- Allowed extensions (same as above).
- The backend performs these checks before saving:
  - filename safe (no `..`, no invalid chars)
  - extension allowed
  - content type starts with `image/` or `video/`
  - available disk space on target drive
- On any failure during create/update the service attempts to delete any partially uploaded files to avoid orphaned files.

Models (summary)
- `ScenicSpotDto` (response):
  - `ScenicSpotName` (string)
  - `Description` (string)
  - `EstimatedVisitPrice` (number)
  - `MediaUrls` (array of string)
  - `ProvinceId` (int)
- `ScenicSpotRequestDto` (create): `ScenicSpotName`, `Description`, `EstimatedVisitPrice`, `Medias` (IFormFile[]), `ProvinceId`.
- `ScenicSpotUpdateRequestDto` (update): `ScenicSpotName`, `Description`, `EstimatedVisitPrice`, `ExistingMedias` (string[]), `NewMedias` (IFormFile[]), `ProvinceId`.
- `RegisterModel`, `LoginModel` — see Auth section above.

Notes & recommendations for frontend
- Always include the `Authorization` header for protected endpoints: `Authorization: Bearer {token}`.
- When updating a scenic spot, first retrieve the current `MediaUrls` (GET by id) and send the ones you want to keep as `ExistingMedias`.
- Send new files using the `NewMedias` form field (multipart file inputs). Use the exact field names used by DTOs.
- Handle `507` (Insufficient Storage) by notifying the user and retrying later or uploading fewer files.
- For security, the backend also validates file contents and disallows suspicious filenames; avoid sending crafted filenames.
- Consider uploading large files to cloud storage (Blob/S3) in the future to avoid local disk limitations.

Extras
- If you want, I can produce a Postman collection or an OpenAPI JSON that matches the current controllers and DTOs in the repository.

