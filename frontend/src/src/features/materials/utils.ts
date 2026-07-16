/**
 * Extract file extension from URL
 */
export function getFileExtension(url: string): string {
  const match = url.match(/\.([^./?#]+)(?:[?#]|$)/);
  return match ? match[1].toLowerCase() : "";
}

/**
 * Open file in a new browser tab
 */
export function openFile(url: string): void {
  window.open(url, "_blank", "noopener,noreferrer");
}

/**
 * Download file to user's device
 */
export function downloadFile(url: string, filename: string): void {
  const anchor = document.createElement("a");
  anchor.href = url;
  anchor.download = filename;
  anchor.target = "_blank";
  anchor.rel = "noopener noreferrer";
  document.body.appendChild(anchor);
  anchor.click();
  document.body.removeChild(anchor);
}
