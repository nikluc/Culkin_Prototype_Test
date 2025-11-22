-Overview-

This Unity memory matching card game challenges players to find pairs of matching cards by flipping them over from a grid layout. The game dynamically adjusts to different grid sizes and maintains smooth gameplay with animated card flips, scoring, multipliers, and saved game state.




-How to Play-

	The grid shows all cards face down after few seconds to memorize.

	Click a card to flip it over and reveal its face.

	Click another card to try and find its matching pair.

	If the cards match, they vanish and add points.

	If not, they flip back, and you try again.

	Every 3 successful pairs increase your score multiplier, missing the match will reset the multiplier.

	Clear all pairs to win and automatically generate new set.



-Designer Configurable Values-

	Grid and Visual Settings (GameManager)

		gridSize (Vector2Int): Dimension of the grid (rows x columns). Increasing grid size increases difficulty.

		uniqueCards (CardData[]): Define the pool of unique cards to be used. Cards repeat if grid size exceeds unique cards.

		defaultCellSize (Vector2): The default size of each card cell in the grid.

		minCellSize (Vector2): The minimum allowed size of each card cell when grid size is large (above 8x8).

		multiplierThreshold (int): Number of matches needed to increase the score multiplier (default is every 3 matches, visual indication will not change (didn't have time to implement dynamic combo)).

	Scoring System

		Base score: Calculated automatically as the sum of number of unique cards plus total grid cells divided by 2.

		Multiplier: Increases by 1 every multiplierThreshold matches to reward streaks.

		Score display: Updated on screen; references via scoreText and multiplierText UI components.

		Combo UI: comboFill array controls visual indicators for ongoing match streaks.

	Audio

		correct, incorrect, win (AudioClip): Assign feedback sounds for match success, failure, and game win.

		Audio managed by AudioManager singleton with SFX audio source pooling.

	Customization

		Grid Responsiveness: Grid layout cell size adjusts automatically if grid is larger than 8x8 to fit the screen.

		Card Animations: Card flips and vanish effects use a separate CardAnimationController script using Unity coroutines.

		Save/Load: Game progress (grid, matches, score, multiplier) is saved in JSON format and loaded on resume.

Usage Notes

	Assign CardData ScriptableObjects with unique IDs and sprites to define distinct cards.

	Use the in-game menu to input desired grid row and column count.

	Ensure grid size produces an even number of cards.

	Use the exposed methods OnMatchFound(), OnCardFlipped(), SaveGame(), and LoadGame() in GameManager for game flow control.

	The score multiplier resets on mismatch streak resets automatically.

Setup for Designers

	Add unique cards: Create and assign card assets in the Inspector.

	Tune cell sizes: Modify defaultCellSize and minCellSize to fit your design aesthetics.

	Assign audio clips: Drag and drop SFX clips for match feedback.

	Multiplier threshod can be modified in the GameManager but visual indication will not change (didn't have time to implement dynamic combo)

	UI hooks: Connect scoreText, multiplierText, and comboFill UI elements.

	Test save/load: Use Save and Load buttons to confirm persistence.
