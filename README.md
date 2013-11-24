This windowsForm code:
	1. Gets raw coordinates from a resistive touch screen
	2. contains various algorithm implementations that convert the
	   obtained raw coordinates into usable coordinates - negating
	   the hardware(touch screen) effects
	   (a) Usable coordinates in terms of its usability for character
	       Recognition
	3. Generates a Pattern from the Coordinates, with respect to the 
	   Pen storkes on the touch screen
	4. Matches the obtained pattern with a database of previously 
	   initialized patterns
	5. When a pattern match is obtained, displays the corresponding 
	   Kannada character using Unicode.
