package day09

import (
	"testing"
)

func TestPart1(t *testing.T) {
	lines := [][]byte{
		[]byte("2199943210"),
		[]byte("3987894921"),
		[]byte("9856789892"),
		[]byte("8767896789"),
		[]byte("9899965678"),
	}

	result := Part1(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 1 not implemented")
	}

	expected := int64(15)
	if result != expected {
		t.Errorf("Error was incorrect, got: %d, want: %d.\n", result, expected)
	}
}

func TestPart2(t *testing.T) {
	lines := [][]byte{
		[]byte("2199943210"),
		[]byte("3987894921"),
		[]byte("9856789892"),
		[]byte("8767896789"),
		[]byte("9899965678"),
	}

	result := Part2(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 2 not implemented")
	}

	expected := int64(1134)
	if result != expected {
		t.Errorf("Error was incorrect, got: %d, want: %d.\n", result, expected)
	}
}
