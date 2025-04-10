package utils

import "testing"

func TestParseNumber(t *testing.T) {
	var expected int64 = 123456
	var input []byte = []byte("123456")

	result := ParseNumber(input)
	if result != expected {
		t.Errorf("Result not equal to expected, got: %d, want %d\n", result, expected)
	}
}
