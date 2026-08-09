---
name: Blaze Health
colors:
  surface: '#f7fafa'
  surface-dim: '#d7dbdb'
  surface-bright: '#f7fafa'
  surface-container-lowest: '#ffffff'
  surface-container-low: '#f1f4f4'
  surface-container: '#ebeeee'
  surface-container-high: '#e6e9e9'
  surface-container-highest: '#e0e3e3'
  on-surface: '#181c1d'
  on-surface-variant: '#3d494b'
  inverse-surface: '#2d3131'
  inverse-on-surface: '#eef1f1'
  outline: '#6d797b'
  outline-variant: '#bcc9cb'
  surface-tint: '#006973'
  primary: '#006670'
  on-primary: '#ffffff'
  primary-container: '#00818d'
  on-primary-container: '#f6feff'
  inverse-primary: '#60d7e6'
  secondary: '#376284'
  on-secondary: '#ffffff'
  secondary-container: '#acd6fe'
  on-secondary-container: '#325d7f'
  tertiary: '#505e60'
  on-tertiary: '#ffffff'
  tertiary-container: '#697779'
  on-tertiary-container: '#f6feff'
  error: '#ba1a1a'
  on-error: '#ffffff'
  error-container: '#ffdad6'
  on-error-container: '#93000a'
  primary-fixed: '#92f1ff'
  primary-fixed-dim: '#60d7e6'
  on-primary-fixed: '#001f23'
  on-primary-fixed-variant: '#004f57'
  secondary-fixed: '#cce5ff'
  secondary-fixed-dim: '#a1cbf2'
  on-secondary-fixed: '#001e31'
  on-secondary-fixed-variant: '#1c4a6b'
  tertiary-fixed: '#d6e5e7'
  tertiary-fixed-dim: '#bac9cb'
  on-tertiary-fixed: '#101e1f'
  on-tertiary-fixed-variant: '#3b494b'
  background: '#f7fafa'
  on-background: '#181c1d'
  surface-variant: '#e0e3e3'
  action-gold: '#F7A400'
  surface-white: '#FFFFFF'
  text-primary: '#001A29'
  text-muted: '#556B77'
typography:
  display-lg:
    fontFamily: Hanken Grotesk
    fontSize: 48px
    fontWeight: '700'
    lineHeight: 56px
    letterSpacing: -0.02em
  display-lg-mobile:
    fontFamily: Hanken Grotesk
    fontSize: 36px
    fontWeight: '700'
    lineHeight: 44px
    letterSpacing: -0.02em
  headline-lg:
    fontFamily: Hanken Grotesk
    fontSize: 32px
    fontWeight: '600'
    lineHeight: 40px
  headline-md:
    fontFamily: Hanken Grotesk
    fontSize: 24px
    fontWeight: '600'
    lineHeight: 32px
  body-lg:
    fontFamily: Hanken Grotesk
    fontSize: 18px
    fontWeight: '400'
    lineHeight: 28px
  body-md:
    fontFamily: Hanken Grotesk
    fontSize: 16px
    fontWeight: '400'
    lineHeight: 24px
  label-md:
    fontFamily: Hanken Grotesk
    fontSize: 14px
    fontWeight: '500'
    lineHeight: 20px
    letterSpacing: 0.01em
  label-sm:
    fontFamily: Hanken Grotesk
    fontSize: 12px
    fontWeight: '600'
    lineHeight: 16px
    letterSpacing: 0.05em
rounded:
  sm: 0.125rem
  DEFAULT: 0.25rem
  md: 0.375rem
  lg: 0.5rem
  xl: 0.75rem
  full: 9999px
spacing:
  base: 8px
  xs: 4px
  sm: 12px
  md: 24px
  lg: 48px
  xl: 80px
  gutter: 24px
  margin-mobile: 16px
  margin-desktop: 64px
---

## Brand & Style

This design system is engineered to project a sense of clinical precision, technological reliability, and absolute trust. It serves a user base that values clarity and efficiency in high-stakes environments, such as health and financial services.

The design style follows a **Modern Corporate** aesthetic with a heavy emphasis on **Minimalism**. By utilizing generous whitespace and a "clinical-tech" lens, the interface remains unobtrusive. Visual weight is reserved strictly for functional wayfinding and critical data points. The atmosphere is quiet, professional, and authoritative, avoiding decorative flourishes in favor of structured hierarchy and high-contrast legibility.

## Colors

The palette is anchored by the signature Teal, used strategically for primary actions and brand presence. The deep Navy (`#003959`) provides the necessary weight for high-level navigation and headers, ensuring a grounded, professional feel.

- **Primary Teal:** Used for primary buttons, active states, and critical brand accents.
- **Secondary Navy:** Used for text headers and high-contrast backgrounds where authority is required.
- **Tertiary Tint:** A soft wash used for background sections, cards, or subtle highlights to break up the white space.
- **Action Gold:** Reserved strictly for high-priority alerts or secondary conversion points that need to stand out from the teal/navy foundation.
- **Neutral/Background:** The primary canvas is Pure White (`#FFFFFF`), with Light Gray (`#F4F7F7`) used for subtle background layering to define different content zones without using heavy borders.

## Typography

Hanken Grotesk provides a modern, geometric clarity that aligns with the high-tech, clinical direction. 

**Application Rules:**
- **Weight Strategy:** Use `600` or `700` for headlines to establish clear hierarchy against the light backgrounds. Use `400` for body text to maintain a light, airy feel.
- **Scale:** Large display sizes should use tighter letter spacing to maintain a "locked-in" professional appearance.
- **Color:** Body text should never be pure black; use the `text-primary` navy for a softer but still high-contrast reading experience. Use `text-muted` for secondary metadata or captions.

## Layout & Spacing

The layout utilizes a **Fixed Grid** system for desktop to ensure content remains centered and readable, moving to a fluid 4-column structure for mobile.

- **Desktop:** 12-column grid, 1200px max-width, 24px gutters.
- **Tablet:** 8-column grid, fluid width, 24px margins.
- **Mobile:** 4-column grid, fluid width, 16px margins.

The spacing rhythm is strictly based on 8px increments to maintain a mathematical, precise layout consistent with the "high-tech" brand profile. Vertical rhythm should prioritize generous "air" between sections (using `xl` spacing) to prevent the clinical look from feeling cramped or cluttered.

## Elevation & Depth

To maintain a clinical and clean look, depth is communicated through **Tonal Layers** and **Ambient Shadows**.

1.  **Level 0 (Base):** Pure White (`#FFFFFF`) or the Tertiary Tint (`#E6F5F7`) for background sections.
2.  **Level 1 (Cards):** Surface White with a subtle 1px border (`#E6F5F7`) or a very soft, diffused shadow.
3.  **Shadow Character:** Shadows should be highly diffused, using a low-opacity Navy tint (`rgba(0, 57, 89, 0.05)`) rather than gray, to keep the palette cohesive.
4.  **Interactive States:** Elements like buttons or active cards should slightly lift on hover, increasing the shadow spread while maintaining the soft, professional feel.

## Shapes

The shape language is **Soft (0.25rem)**. This provides a subtle modern touch that breaks the harshness of a purely clinical grid without becoming overly "friendly" or "playful." 

- **Primary Components:** (Buttons, Inputs) use 4px (`rounded-sm`).
- **Containers:** (Cards, Modals) use 8px (`rounded-lg`) to provide a clear container-level distinction.
- **Selection Indicators:** (Chips) may use a full pill shape for high-contrast visibility, but generally, the system adheres to the 4px standard.

## Components

### Buttons
- **Primary:** Solid Teal (`#009FAE`) with white text. 4px border radius. No shadow in static state; subtle shadow on hover.
- **Secondary:** Transparent background with a Teal 2px border.
- **Ghost:** Navy text on transparent background, used for low-priority actions.

### Input Fields
- **Default State:** White background, 1px border (`#E6F5F7`). 
- **Focus State:** 2px Teal border with a very faint Teal outer glow (2px blur).
- **Labels:** Use `label-md` in Secondary Navy for high legibility.

### Cards
- White background, 8px radius, with a subtle 1px border. Used to group clinical data or insurance details. Ensure internal padding is consistent with the `md` spacing unit (24px).

### Chips & Tags
- Used for status indicators (e.g., "Active," "Pending"). Use the Tertiary Tint (`#E6F5F7`) with Primary Teal text for a "low-noise" but clearly visible state.

### Lists
- Use horizontal dividers in a very faint gray (`#F4F7F7`). Do not use zebra striping; instead, use generous vertical padding to separate list items.