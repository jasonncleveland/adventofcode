package day17

import (
	"testing"
)

func TestPart1(t *testing.T) {
	lines := [][]byte{
		[]byte("target area: x=20..30, y=-10..-5"),
	}

	result := Part1(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 1 not implemented")
	}

	expected := int64(45)
	if result != expected {
		t.Errorf("Result was incorrect, got: %v, want: %v.\n", result, expected)
	}
}

func TestPart2(t *testing.T) {
	lines := [][]byte{
		[]byte("target area: x=20..30, y=-10..-5"),
	}

	result := Part2(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 2 not implemented")
	}

	expected := int64(112)
	if result != expected {
		t.Errorf("Result was incorrect, got: %v, want: %v.\n", result, expected)
	}
}
