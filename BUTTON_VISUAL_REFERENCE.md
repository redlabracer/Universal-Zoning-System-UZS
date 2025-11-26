# Universal Theme Button - Visual Reference

## Button Design Specification

### Appearance

```
??????????????????????????????????????????????????????????
?                  ZONING TOOLBAR                        ?
?                                                        ?
?  Theme:  [UP] [?] [MM] [UP] [CN] [DE] [EE] [FR] ...  ?
?           ?                                            ?
?           ?                                            ?
?      UNIVERSAL PACK                                    ?
?     (Your new button!)                                 ?
??????????????????????????????????????????????????????????
```

### Button Details

**Default State:**
```
????????????
?    UP    ?  ? Purple gradient background
????????????     Light purple border
  Size: Matches other theme buttons
  Color: #6B46C1 ? #4299E1 gradient
  Border: 2px solid #805AD5
```

**Hover State:**
```
????????????
?    UP    ?  ? Slightly larger (105%)
????????????     Purple glow effect
  Scale: 1.05x
  Shadow: 0 4px 12px rgba(107, 70, 193, 0.5)
```

**Active State (Filtering On):**
```
????????????
?    UP    ?  ? Darker purple
????????????     Indicates filtering active
  Color: #553C9A ? #2B6CB0 gradient
  Border: 2px solid #6B46C1
```

---

## Color Palette

### Primary Colors
- **Gradient Start**: `#6B46C1` (Purple)
- **Gradient End**: `#4299E1` (Blue)
- **Border**: `#805AD5` (Light Purple)

### Active State Colors
- **Gradient Start**: `#553C9A` (Dark Purple)
- **Gradient End**: `#2B6CB0` (Dark Blue)
- **Border**: `#6B46C1` (Purple)

### Text
- **Color**: `white`
- **Font Weight**: `bold`
- **Font Size**: `14px`
- **Text Shadow**: `0 1px 3px rgba(0,0,0,0.5)`

---

## Animation Specifications

### Hover Animation
```
Transition: all 0.2s ease
Transform: scale(1.05)
Box Shadow: 0 4px 12px rgba(107, 70, 193, 0.5)
```

### Click Animation
```
Immediate color change
No transition on click for responsiveness
```

---

## Layout Integration

### Before Button Added:
```
Theme Section:
[?] [MM] [UP] [CN] [DE] [EE] [FR] [JP] [NE] [NL] [SW] [UK]
 ?
 All theme packs in order
```

### After Button Added:
```
Theme Section:
[UP] [?] [MM] [UP] [CN] [DE] [EE] [FR] [JP] [NE] [NL] [SW] [UK]
 ?
 Universal Pack added at the front!
```

---

## Interaction States

### State 1: Ready (Inactive)
```
Visual:   Bright purple gradient
Zones:    All zones visible
Buttons:  All theme buttons active
Status:   Ready to filter
```

### State 2: Filtering (Active)
```
Visual:   Dark purple gradient
Zones:    Only Universal zones visible
Buttons:  Theme buttons dimmed (30% opacity)
Status:   Filter applied
```

### State 3: Hover
```
Visual:   Scaled up 5%, glowing
Zones:    Current state maintained
Buttons:  Current state maintained
Status:   Interactive feedback
```

---

## Responsive Behavior

### Button adapts to:
- **Screen Resolution**: Scales with UI
- **Theme Section Size**: Fits within container
- **Other Theme Buttons**: Matches their size
- **Zoom Level**: Maintains proportions

---

## Comparison with Game's Theme Buttons

### Standard Theme Button:
```
????????????
?    NA    ?  ? Plain style
????????????     Game's default
```

### Universal Pack Button:
```
????????????
?    UP    ?  ? Gradient style
????????????     Custom purple theme
                 Stands out visually!
```

---

## Visual Hierarchy

```
Priority Level:
1. Universal Pack Button ??
                          ?? Same visual weight
2. Other Theme Buttons ????

When Active:
1. Universal Pack Button ???? Bright/Dark
2. Other Theme Buttons ?????? Dimmed
```

---

## Accessibility

### Contrast Ratios:
- **Text on Button**: High contrast (white on purple)
- **Button vs Background**: Clear separation
- **Active vs Inactive**: Noticeable difference

### Visual Indicators:
- **Color**: Purple = Universal
- **Text**: "UP" = Universal Pack
- **Tooltip**: "Universal Pack - Click to show only Universal zones"

---

## Size Specifications

### Dimensions:
- **Width**: Auto (fits text + padding)
- **Height**: Matches theme buttons (~36-40px estimated)
- **Padding**: 8px horizontal, 12px vertical
- **Border Radius**: 4px
- **Border Width**: 2px

### Spacing:
- **Margin**: 0 5px (left and right)
- **Between Buttons**: Consistent with game's spacing

---

## Example Screenshots Composition

### Screenshot 1: Button in Context
```
???????????????????????????????????????
?  CITIES: SKYLINES II                ?
?                                     ?
?  ???????????????????????????????  ?
?  ? Theme: [UP] [?] [MM] [UP]  ?  ? ? Show this!
?  ?                             ?  ?
?  ? Zoning Options:             ?  ?
?  ? [Universal Low Res] [....]  ?  ?
?  ???????????????????????????????  ?
???????????????????????????????????????
```

### Screenshot 2: Button Active (Filtering)
```
???????????????????????????????????????
?  Only Universal Zones Visible:     ?
?                                     ?
?  [UP] ??? Active (darker)           ?
?  [?] [MM] [UP] ... ??? Dimmed      ?
?                                     ?
?  Universal Low Res    ?            ?
?  Universal Row        ?            ?
?  Universal Medium     ?            ?
?  NA Low Res          (hidden)      ?
?  EU Low Res          (hidden)      ?
???????????????????????????????????????
```

### Screenshot 3: Hover Effect
```
???????????????????????????????????????
?  Theme: [UP] [?] [MM]              ?
?          ?                          ?
?     ??????????                     ?
?     ? Purple ?                     ?
?     ?  glow  ?                     ?
?     ??????????                     ?
?     Slightly larger on hover       ?
???????????????????????????????????????
```

---

## Print-Ready Description for Publishing

**Visual Description:**
> The Universal Pack button features a stunning purple-to-blue gradient design that instantly catches the eye. Positioned at the front of the theme section, it displays "UP" in bold white text. When you hover over it, the button smoothly scales up and emits a subtle purple glow. Clicking it transforms the button to a darker shade, indicating that only Universal zones are now visible.

**Color Philosophy:**
> Purple represents the fusion of North American (traditionally blue) and European (traditionally gold/brown) themes, symbolizing the universal nature of these mixed-style zones.

---

## CSS-Style Specification (For Reference)

```css
.universal-theme-button {
  /* Default State */
  background: linear-gradient(135deg, #6B46C1 0%, #4299E1 100%);
  border: 2px solid #805AD5;
  border-radius: 4px;
  padding: 8px 12px;
  color: white;
  font-weight: bold;
  font-size: 14px;
  text-shadow: 0 1px 3px rgba(0,0,0,0.5);
  cursor: pointer;
  transition: all 0.2s ease;
}

.universal-theme-button:hover {
  transform: scale(1.05);
  box-shadow: 0 4px 12px rgba(107, 70, 193, 0.5);
}

.universal-theme-button.active {
  background: linear-gradient(135deg, #553C9A 0%, #2B6CB0 100%);
  border-color: #6B46C1;
}
```

---

## Marketing Visuals Suggestions

### For Mod Page Banner:
- Show the button prominently
- Use purple/blue theme
- Include tagline: "One Click, Infinite Variety"

### For Screenshots:
1. **Hero Shot**: Close-up of button with tooltip
2. **Before/After**: All zones vs. Universal only
3. **In Action**: Player using the filter
4. **Integration**: Button among other theme buttons
5. **Results**: Mixed NA/EU buildings from Universal zones

### For Animated GIF:
```
Frame 1: All zones visible, button inactive
Frame 2: Cursor hovers over button (glow effect)
Frame 3: Click animation
Frame 4: Filter applies, only Universal zones shown
Frame 5: Button now in active state (darker)
Loop back to Frame 1
```

---

This visual reference provides all the details needed to recreate, document, or showcase the Universal Theme Button!
