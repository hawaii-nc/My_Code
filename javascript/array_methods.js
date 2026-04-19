const numbers = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

// map: double each number
const doubled = numbers.map((n) => n * 2);
console.log("Doubled:", doubled);

// filter: keep only even numbers
const evens = numbers.filter((n) => n % 2 === 0);
console.log("Evens:", evens);

// reduce: sum all numbers
const sum = numbers.reduce((acc, n) => acc + n, 0);
console.log("Sum:", sum);
