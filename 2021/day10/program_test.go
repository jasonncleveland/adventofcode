package day10

import (
	"testing"
)

func TestPart1(t *testing.T) {
	lines := [][]byte{
		[]byte("[({(<(())[]>[[{[]{<()<>>"),
		[]byte("[(()[<>])]({[<{<<[]>>("),
		[]byte("{([(<{}[<>[]}>{[]{[(<()>"),
		[]byte("(((({<>}<{<{<>}{[]{[]{}"),
		[]byte("[[<[([]))<([[{}[[()]]]"),
		[]byte("[{[{({}]{}}([{[{{{}}([]"),
		[]byte("{<[[]]>}<{[{[{[]{()[[[]"),
		[]byte("[<(<(<(<{}))><([]([]()"),
		[]byte("<{([([[(<>()){}]>(<<{{"),
		[]byte("<{([{{}}[<[[[<>{}]]]>[]]"),
	}

	result := Part1(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 1 not implemented")
	}

	expected := int64(26397)
	if result != expected {
		t.Errorf("Error was incorrect, got: %d, want: %d.\n", result, expected)
	}
}

func TestPart2(t *testing.T) {
	lines := [][]byte{
		[]byte("[({(<(())[]>[[{[]{<()<>>"),
		[]byte("[(()[<>])]({[<{<<[]>>("),
		[]byte("{([(<{}[<>[]}>{[]{[(<()>"),
		[]byte("(((({<>}<{<{<>}{[]{[]{}"),
		[]byte("[[<[([]))<([[{}[[()]]]"),
		[]byte("[{[{({}]{}}([{[{{{}}([]"),
		[]byte("{<[[]]>}<{[{[{[]{()[[[]"),
		[]byte("[<(<(<(<{}))><([]([]()"),
		[]byte("<{([([[(<>()){}]>(<<{{"),
		[]byte("<{([{{}}[<[[[<>{}]]]>[]]"),
	}

	result := Part2(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 2 not implemented")
	}

	expected := int64(288957)
	if result != expected {
		t.Errorf("Error was incorrect, got: %d, want: %d.\n", result, expected)
	}
}
