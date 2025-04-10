package day14

import (
	"testing"
)

func TestPart1(t *testing.T) {
	t.Skip()
	lines := [][]byte{
		[]byte("00000"),
		[]byte("00100"),
		[]byte("01110"),
		[]byte("00100"),
		[]byte("00000"),
	}

	result := Part1(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 1 not implemented")
	}

	expected := int64(0)
	if result != expected {
		t.Errorf("Result was incorrect, got: %v, want: %v.\n", result, expected)
	}
}

func TestPart2(t *testing.T) {
	t.Skip()
	lines := [][]byte{
		[]byte("00000"),
		[]byte("00100"),
		[]byte("01110"),
		[]byte("00100"),
		[]byte("00000"),
	}

	result := Part2(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 2 not implemented")
	}

	expected := int64(0)
	if result != expected {
		t.Errorf("Result was incorrect, got: %v, want: %v.\n", result, expected)
	}
}

func TestParseInput(t *testing.T) {
	expected := [][]int64{
		{0, 0, 0, 0, 0},
		{0, 0, 1, 0, 0},
		{0, 1, 1, 1, 0},
		{0, 0, 1, 0, 0},
		{0, 0, 0, 0, 0},
	}
	lines := [][]byte{
		[]byte("00000"),
		[]byte("00100"),
		[]byte("01110"),
		[]byte("00100"),
		[]byte("00000"),
	}

	result := ParseInput(lines)
	if len(result) != len(expected) {
		t.Fatalf("Length of result is not equal to the expected length, got: %d, want %d\n", len(result), len(expected))
	}
	for i := range result {
		if len(result[i]) != len(expected[i]) {
			t.Fatalf("Length of result is not equal to the expected length, got: %d, want %d\n", len(result[i]), len(expected[i]))
		}
		for j := range result[i] {
			if result[i][j] != expected[i][j] {
				t.Errorf("Value at index %d in result is not equal to expected, got: %d, want: %d\n", i, result[i][j], expected[i][j])
			}
		}
	}
}
