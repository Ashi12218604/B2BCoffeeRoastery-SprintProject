@echo off
echo.
echo --- 1. REGISTERING USER ---
curl.exe -s -X POST "http://127.0.0.1:7101/api/auth/register-client" -H "Content-Type: application/json" -d "{\"email\":\"final_demo_2026@gmail.com\",\"password\":\"Password123!\",\"businessName\":\"E&B Demo\",\"address\":\"Coffee City\",\"phoneNumber\":\"1234567890\"}"

echo.
echo.
echo --- 2. LOGGING IN ---
curl.exe -s -X POST "http://127.0.0.1:7101/api/auth/login" -H "Content-Type: application/json" -d "{\"email\":\"final_demo_2026@gmail.com\",\"password\":\"Password123!\"}" > login_response.json
type login_response.json

echo.
echo.
echo --- 3. PLACING ORDER (WITH CORRELATION ID TEST) ---
REM Extracting token manually is hard in batch, I will just hardcode the token if I can read the file next step.
