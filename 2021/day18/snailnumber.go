package day18

import (
	"math"
	"os"
	"strconv"
	"strings"
)

type SnailNumberSide int

const (
	Root      SnailNumberSide = 0
	LeftSide  SnailNumberSide = 1
	RightSide SnailNumberSide = 2
)

type SnailNumber struct {
	Value  int64
	Side   SnailNumberSide
	Parent *SnailNumber
	Left   *SnailNumber
	Right  *SnailNumber
}

// Struct String Representations

func (node *SnailNumber) ColourText(builder *strings.Builder, operation func()) {
	// Set the AOC_DEBUG environment variable to "true" for coloured text
	if os.Getenv("AOC_DEBUG") != "true" {
		operation()
		return
	}

	switch node.Side {
	case LeftSide:
		builder.WriteString("\033[1;35m")
	case RightSide:
		builder.WriteString("\033[1;36m")
	default:
		builder.WriteString("\033[1;31m")
	}
	operation()
	builder.WriteString("\033[0m")
}

func (node *SnailNumber) BuildStringRec(builder *strings.Builder) {
	if node == nil {
		builder.WriteString("[]")
		return
	}

	if node.Value > -1 {
		node.ColourText(builder, func() {
			builder.WriteString(strconv.Itoa(int(node.Value)))
		})
	} else {
		node.ColourText(builder, func() {
			builder.WriteRune('[')
		})

		node.Left.BuildStringRec(builder)

		node.ColourText(builder, func() {
			builder.WriteRune(',')
		})

		node.Right.BuildStringRec(builder)

		node.ColourText(builder, func() {
			builder.WriteRune(']')
		})
	}
}

func (node *SnailNumber) ToString() string {
	var builder strings.Builder
	node.BuildStringRec(&builder)
	return builder.String()
}

// Struct Operations

func (node *SnailNumber) ExplodeLeftValue() {
	current := node

	// Go up until we are no longer on the left side
	for current.Side == LeftSide {
		current = current.Parent
	}

	// Stop if we have reached the root node
	if current.Side == 0 {
		return
	}

	// Switch to the right node and traverse until value is found
	current = current.Parent.Left

	for current.Right != nil {
		current = current.Right
	}

	// Add exploded value to found node value
	current.Value += node.Left.Value
}

func (node *SnailNumber) ExplodeRightValue() {
	current := node

	// Go up until we are no longer on the right side
	for current.Side == RightSide {
		current = current.Parent
	}

	// Stop if we have reached the root node
	if current.Side == 0 {
		return
	}

	// Switch to the left node and traverse until value is found
	current = current.Parent.Right

	for current.Left != nil {
		current = current.Left
	}

	// Add exploded value to found node value
	current.Value += node.Right.Value
}

func (node *SnailNumber) Explode(depth int) bool {
	if node.Left == nil {
		return false
	}

	if depth == 4 {
		node.ExplodeLeftValue()
		node.ExplodeRightValue()
		node.Value = 0
		node.Left = nil
		node.Right = nil
		return true
	}

	return node.Left.Explode(depth+1) || node.Right.Explode(depth+1)
}

func (node *SnailNumber) Split() bool {
	if node.Value >= 10 {
		left := &SnailNumber{
			Value:  int64(math.Floor(float64(node.Value) / 2)),
			Side:   LeftSide,
			Parent: node,
		}
		right := &SnailNumber{
			Value:  int64(math.Ceil(float64(node.Value) / 2)),
			Side:   RightSide,
			Parent: node,
		}

		node.Value = -1
		node.Left = left
		node.Right = right

		return true
	}

	if node.Value == -1 {
		return node.Left.Split() || node.Right.Split()
	}
	return false
}

func (node *SnailNumber) Reduce() {
	for {
		// Attempt to explode pair
		if node.Explode(0) {
			continue
		}

		// Attempt to split pair if none were exploded
		if node.Split() {
			continue
		}

		// Stop if nothing was exploded or split
		break
	}
}

func (node *SnailNumber) Magnitude() int64 {
	if node.Value != -1 {
		return node.Value
	}

	var left int64 = node.Left.Magnitude()
	var right int64 = node.Right.Magnitude()

	return 3*left + 2*right
}

// Non-Struct Functions

func Parse(line []byte) *SnailNumber {
	index := 0
	return ParseRec(line, &index, Root)
}

func ParseRec(line []byte, index *int, side SnailNumberSide) *SnailNumber {
	node := SnailNumber{Value: -1, Side: side}

	// We will always start with the index being where the opening left bracket is
	*index++
	if line[*index] == '[' {
		// Left side is recursive
		node.Left = ParseRec(line, index, LeftSide)
		node.Left.Parent = &node
	} else {
		// Left side is number
		node.Left = &SnailNumber{Value: int64(line[*index] - '0'), Side: LeftSide, Parent: &node}
	}

	// Increment one to skip the comma separator
	*index++
	*index++

	if line[*index] == '[' {
		// Right side is recursive
		node.Right = ParseRec(line, index, RightSide)
		node.Right.Parent = &node
	} else {
		// Right side is number
		node.Right = &SnailNumber{Value: int64(line[*index] - '0'), Side: RightSide, Parent: &node}
	}
	*index++

	return &node
}

func Add(left *SnailNumber, right *SnailNumber) *SnailNumber {
	if left == nil {
		return right
	}

	root := &SnailNumber{
		Value: -1,
		Left:  left,
		Right: right,
	}
	left.Parent = root
	left.Side = LeftSide
	right.Parent = root
	right.Side = RightSide

	root.Reduce()

	return root
}
