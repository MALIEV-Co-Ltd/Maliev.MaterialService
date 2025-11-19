# Performance Tests

This directory contains performance test scripts using [k6](https://k6.io/).

## Prerequisites

1. Install k6:
   - Windows (winget): `winget install k6`
   - macOS (brew): `brew install k6`
   - Linux: `sudo apt-get install k6`

## Running the Tests

1. Ensure the API is running locally (e.g., `http://localhost:5007`).
2. Run the load test:
   ```bash
   k6 run load-test.js
   ```

## Scenarios

- **load-test.js**: Simulates a basic load of 20 concurrent users fetching the materials list.
