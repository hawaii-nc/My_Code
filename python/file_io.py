def write_file(filename, content):
    """Write content to a file."""
    with open(filename, "w") as f:
        f.write(content)
    print(f"Written to {filename}")


def read_file(filename):
    """Read and return content from a file."""
    with open(filename, "r") as f:
        return f.read()


if __name__ == "__main__":
    filename = "example.txt"
    write_file(filename, "Hello from file_io.py!\nSecond line here.")
    content = read_file(filename)
    print(f"Read from {filename}:\n{content}")
