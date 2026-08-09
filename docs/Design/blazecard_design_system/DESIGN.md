---
name: BlazeCard Design System
colors:
  surface: '#0e1417'
  surface-dim: '#0e1417'
  surface-bright: '#343a3d'
  surface-container-lowest: '#090f12'
  surface-container-low: '#161c1f'
  surface-container: '#1a2024'
  surface-container-high: '#252b2e'
  surface-container-highest: '#303639'
  on-surface: '#dee3e7'
  on-surface-variant: '#bcc9cb'
  inverse-surface: '#dee3e7'
  inverse-on-surface: '#2b3135'
  outline: '#869395'
  outline-variant: '#3d494b'
  surface-tint: '#60d7e6'
  primary: '#60d7e6'
  on-primary: '#00363c'
  primary-container: '#03a0af'
  on-primary-container: '#002f34'
  inverse-primary: '#006973'
  secondary: '#a1cbf2'
  on-secondary: '#003351'
  secondary-container: '#1f4d6e'
  on-secondary-container: '#93bde3'
  tertiary: '#ffb953'
  on-tertiary: '#452b00'
  tertiary-container: '#c68300'
  on-tertiary-container: '#3c2500'
  error: '#ffb4ab'
  on-error: '#690005'
  error-container: '#93000a'
  on-error-container: '#ffdad6'
  primary-fixed: '#92f1ff'
  primary-fixed-dim: '#60d7e6'
  on-primary-fixed: '#001f23'
  on-primary-fixed-variant: '#004f57'
  secondary-fixed: '#cce5ff'
  secondary-fixed-dim: '#a1cbf2'
  on-secondary-fixed: '#001e31'
  on-secondary-fixed-variant: '#1c4a6b'
  tertiary-fixed: '#ffddb4'
  tertiary-fixed-dim: '#ffb953'
  on-tertiary-fixed: '#291800'
  on-tertiary-fixed-variant: '#633f00'
  background: '#0e1417'
  on-background: '#dee3e7'
  surface-variant: '#303639'
  surface-charcoal: '#1A2327'
  surface-elevated: '#242E33'
  hbf-white: '#FFFFFF'
  hbf-light-gray: '#F8F8F8'
typography:
  headline-xl:
    fontFamily: Hanken Grotesk
    fontSize: 48px
    fontWeight: '700'
    lineHeight: 56px
    letterSpacing: -0.02em
  headline-lg:
    fontFamily: Hanken Grotesk
    fontSize: 32px
    fontWeight: '600'
    lineHeight: 40px
  headline-lg-mobile:
    fontFamily: Hanken Grotesk
    fontSize: 24px
    fontWeight: '600'
    lineHeight: 32px
  body-md:
    fontFamily: Inter
    fontSize: 16px
    fontWeight: '400'
    lineHeight: 24px
  body-sm:
    fontFamily: Inter
    fontSize: 14px
    fontWeight: '400'
    lineHeight: 20px
  label-caps:
    fontFamily: JetBrains Mono
    fontSize: 12px
    fontWeight: '500'
    lineHeight: 16px
    letterSpacing: 0.05em
  code-block:
    fontFamily: JetBrains Mono
    fontSize: 13px
    fontWeight: '400'
    lineHeight: 20px
rounded:
  sm: 0.125rem
  DEFAULT: 0.25rem
  md: 0.375rem
  lg: 0.5rem
  xl: 0.75rem
  full: 9999px
spacing:
  unit: 4px
  gutter: 24px
  margin-mobile: 16px
  margin-desktop: 40px
  sidebar-width: 280px
  editor-panel: 360px
---

## Brand & Style

The design system is a high-performance Fintech framework designed for precision, clarity, and trust. It facilitates a professional "Card Previewer" experience where functional utility meets premium aesthetics. 

The style is **Modern Corporate**, leaning into **Minimalism** with a **Glassmorphic** touch for interactive elements. It prioritizes information density without sacrificing legibility. The interface should feel technical yet accessible, utilizing sharp alignment, subtle translucency for overlays, and high-contrast accents to guide the user through complex financial workflows and visual editing tasks.

## Colors

The palette is derived from HBF's heritage teal and blue, adapted for a high-end Fintech environment. 

### Theme Architecture
This design system defaults to a **Dark Mode** to emphasize the vibrant teal accents and the "glow" of card previews. 
- **Primary (#009FAE):** Used for primary actions, success states, and active selection borders.
- **Secondary (#003959):** Used for sidebar backgrounds, header bars, and deep containers to provide structural grounding.
- **Tertiary (#F7A400):** Reserved for high-priority alerts, warning states, or specialized "Gold" card tier previews.
- **Neutral/Background (#12181B):** A deep charcoal that provides better contrast for financial data than pure black.

For Light Mode applications (e.g., printed statements or exported reports), use `#FFFFFF` as the base with `#F8F8F8` for subtle sectional division.

## Typography

The typography strategy separates brand expression from functional utility.

- **Headlines (Hanken Grotesk):** Provides a sharp, contemporary "tech" feel. Use for page titles and major card section headers.
- **Body (Inter):** Chosen for its exceptional legibility in data-heavy tables and form fields.
- **Labels & Technical Data (JetBrains Mono):** Used for card metadata (CVV, Expiry), JSON code blocks, and system status labels to evoke a sense of precision and "builder" tools.

## Layout & Spacing

The layout utilizes a **Fixed Sidebar + Fluid Workspace** model. 

- **Desktop:** A three-pane layout. Left (280px) for navigation; Center (Fluid) for the live card preview; Right (360px) for the configuration tools and JSON editor.
- **Grid:** Use a 12-column grid for the central workspace. Elements should align to a strict 4px baseline shift to maintain visual rhythm.
- **Breakpoints:** 
  - **Mobile (<768px):** Stacked vertical layout. Sidebar becomes a bottom-sheet or hamburger menu. Card preview scales to width minus 32px margin.
  - **Tablet (768px - 1280px):** Sidebar collapses to icons. Editor panel becomes a toggleable drawer.

## Elevation & Depth

This design system uses **Tonal Layering** combined with **Inner Glows** rather than heavy drop shadows.

- **Base Layer:** `#12181B` (The deep canvas).
- **Mid Layer:** `#1A2327` (Used for the main card container and navigation panels).
- **Top Layer:** `#242E33` (Used for active modals, popovers, and floating tooltips).
- **Interactive Depth:** Buttons and active input fields utilize a subtle 1px border of `#009FAE` with a 10% opacity outer glow of the same color to simulate a "lit" digital interface.
- **Glassmorphism:** Use a `backdrop-filter: blur(12px)` with a 60% opacity fill of `#1A2327` for headers and contextual menus to maintain a sense of space.

## Shapes

The shape language is **Soft** but disciplined. 

- **Standard Elements:** Inputs, buttons, and small containers use a **0.25rem (4px)** radius to maintain a professional, slightly technical edge.
- **Card Previews:** To mimic physical credit cards, preview elements should use a specific **rounded-xl (0.75rem / 12px)** radius.
- **Iconography:** Use linear, 2px stroke-width icons with slight corner rounding to match the typography's geometric nature.

## Components

### Buttons
- **Primary:** Solid `#009FAE` with white text. High-contrast.
- **Ghost:** Transparent background with `#009FAE` 1px border.
- **Action:** For "Delete" or "Reset," use a subtle `#F7A400` outline to indicate caution without the aggression of pure red.

### Form Inputs
- **Field Style:** Deep background (`#12181B`) with a 1px border (`#242E33`). Upon focus, the border transitions to Primary Teal with a subtle glow.
- **Labels:** Use `label-caps` typography (JetBrains Mono) placed above the field in 60% opacity white.

### Code Blocks & Editors
- Use a monochromatic dark background. Syntax highlighting should strictly use the Teal (#009FAE) and Gold (#F7A400) from the brand palette to maintain cohesion.

### Visual Preview Cards
- Must include a CSS-based "shimmer" or "glint" effect to simulate physical card materials (matte vs. gloss).
- Typography on cards should use `Inter` for legibility, regardless of the system-wide font.

### Progress Steppers
- Use thin Teal lines with 8px circular nodes to indicate stages of card configuration.