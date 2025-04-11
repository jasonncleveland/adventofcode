package day15

import (
	"testing"
)

func TestPart1(t *testing.T) {
	lines := [][]byte{
		[]byte("1163751742"),
		[]byte("1381373672"),
		[]byte("2136511328"),
		[]byte("3694931569"),
		[]byte("7463417111"),
		[]byte("1319128137"),
		[]byte("1359912421"),
		[]byte("3125421639"),
		[]byte("1293138521"),
		[]byte("2311944581"),
	}

	result := Part1(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 1 not implemented")
	}

	expected := int64(40)
	if result != expected {
		t.Errorf("Result was incorrect, got: %v, want: %v.\n", result, expected)
	}
}

func TestPart2(t *testing.T) {
	lines := [][]byte{
		[]byte("1163751742"),
		[]byte("1381373672"),
		[]byte("2136511328"),
		[]byte("3694931569"),
		[]byte("7463417111"),
		[]byte("1319128137"),
		[]byte("1359912421"),
		[]byte("3125421639"),
		[]byte("1293138521"),
		[]byte("2311944581"),
	}

	result := Part2(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 2 not implemented")
	}

	expected := int64(315)
	if result != expected {
		t.Errorf("Result was incorrect, got: %v, want: %v.\n", result, expected)
	}
}
