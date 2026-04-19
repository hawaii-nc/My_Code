def binary_search(arr, target):
    """Search for target in a sorted list. Returns index or -1 if not found."""
    left, right = 0, len(arr) - 1
    while left <= right:
        mid = (left + right) // 2
        if arr[mid] == target:
            return mid
        elif arr[mid] < target:
            left = mid + 1
        else:
            right = mid - 1
    return -1


if __name__ == "__main__":
    data = [1, 3, 5, 7, 9, 11, 13, 15, 17, 19]
    target = 11
    index = binary_search(data, target)
    if index != -1:
        print(f"Found {target} at index {index}")
    else:
        print(f"{target} not found in list")
