# Fix: Storage Path Parsing for Quiz Generation

## Problem
HTTP 404 when downloading files from storage during quiz generation.

**Stored URL:**  
`https://jadara-hub.b-cdn.net/permanent/Lesson/81e64f1b89a5436f8f394c8bb2daf09e.pdf`

**Wrong download attempt:**  
`storage.bunnycdn.com/jadara-hub/Lesson/file.pdf` ?

**Correct download:**  
`storage.bunnycdn.com/jadara-hub/permanent/Lesson/file.pdf` ?

## Solution
Changed path parsing from `segments[1..^1]` to `segments[..^1]` to include ALL path segments except filename.

## Result
- Folder: `"permanent/Lesson"` ?
- Full path: `jadara-hub/permanent/Lesson/file.pdf` ?

## Action Required
**Restart the application** - changes cannot be hot-reloaded.
