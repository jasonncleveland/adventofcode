package day21

import (
	"testing"
)

func TestPart1(t *testing.T) {
	lines := [][]byte{
		[]byte("Player 1 starting position: 4"),
		[]byte("Player 2 starting position: 8"),
	}

	result := Part1(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 1 not implemented")
	}

	expected := int64(739785)
	if result != expected {
		t.Errorf("Result was incorrect, got: %v, want: %v.\n", result, expected)
	}
}

func TestPart2(t *testing.T) {
	lines := [][]byte{
		[]byte("Player 1 starting position: 4"),
		[]byte("Player 2 starting position: 8"),
	}

	result := Part2(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 2 not implemented")
	}

	expected := int64(444356092776315)
	if result != expected {
		t.Errorf("Result was incorrect, got: %v, want: %v.\n", result, expected)
	}
}
