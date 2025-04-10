package day02

import (
	"testing"
)

func TestPart1(t *testing.T) {
	instructions := []instruction{
		{"forward", 5},
		{"down", 5},
		{"forward", 8},
		{"up", 3},
		{"down", 8},
		{"forward", 2},
	}
	result := Part1(instructions)

	expected := int64(150)
	if result != expected {
		t.Errorf("Error was incorrect, got: %d, want: %d.\n", result, expected)
	}
}

func TestPart2(t *testing.T) {
	instructions := []instruction{
		{"forward", 5},
		{"down", 5},
		{"forward", 8},
		{"up", 3},
		{"down", 8},
		{"forward", 2},
	}
	result := Part2(instructions)

	expected := int64(900)
	if result != expected {
		t.Errorf("Error was incorrect, got: %d, want: %d.\n", result, expected)
	}
}

func TestParseInput(t *testing.T) {
	instructions := []instruction{
		{"forward", 5},
		{"down", 5},
		{"forward", 8},
		{"up", 3},
		{"down", 8},
		{"forward", 2},
	}
	lines := [][]byte{
		[]byte("forward 5"),
		[]byte("down 5"),
		[]byte("forward 8"),
		[]byte("up 3"),
		[]byte("down 8"),
		[]byte("forward 2"),
	}

	result := ParseInput(lines)
	if len(result) != len(instructions) {
		t.Fatalf("Length of result %d is not equal to the expected length %d\n", len(result), len(instructions))
	}
	for i := range result {
		if result[i] != instructions[i] {
			t.Errorf("Instruction at index %d in result %v is not equal to expected %v\n", i, result[i], instructions[i])
		}
	}
}
