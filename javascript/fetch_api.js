// Example: Fetching data from a public API
async function fetchUser(userId) {
  const response = await fetch(
    `https://jsonplaceholder.typicode.com/users/${userId}`
  );
  if (!response.ok) {
    throw new Error(`HTTP error: ${response.status}`);
  }
  const user = await response.json();
  return user;
}

// Usage (run in a browser or Node.js 18+)
fetchUser(1)
  .then((user) => console.log("User:", user.name, "|", user.email))
  .catch((err) => console.error("Error:", err.message));
