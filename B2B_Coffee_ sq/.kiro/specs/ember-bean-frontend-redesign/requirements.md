# Requirements Document

## Introduction

Ember & Bean is a premium coffee roastery web app built in Angular. The frontend currently suffers from a visual disconnect: the auth pages use a frosted silver/white card treatment that feels clinical against the warm roastery background image, and the dashboard areas lack the tactile warmth and interactivity expected of a premium brand. This redesign addresses that disconnect by replacing cold card backgrounds with warm, brand-cohesive treatments, adding micro-animations and hover interactivity across all pages, and ensuring visual consistency from auth through admin and client portals — all without touching the backend or changing existing colors and fonts.

## Glossary

- **UI**: The Angular frontend application located in `Frontend/EmberBean/src/`
- **Auth_Pages**: The Login, Register, and Verify OTP components under `features/auth/`
- **Admin_Pages**: The Admin Layout, Dashboard, Orders, Products, Inventory, Deliveries, and Pending Clients components under `features/admin/`
- **Client_Pages**: The Client Layout, Dashboard, Catalog, and My Orders components under `features/client/`
- **Card**: Any panel, modal, or container element that groups content visually (e.g., the login panel, dashboard stat cards, product cards)
- **Warm_Card_Treatment**: A card background using dark roast browns, warm amber tones, or semi-transparent espresso/copper overlays — as opposed to white or silver frosted glass
- **Design_System**: The global SCSS tokens defined in `src/styles/_variables.scss`, `_glassmorphism.scss`, `_animations.scss`, and `_typography.scss`
- **Micro_Animation**: A short, subtle CSS transition or keyframe animation (duration 150ms–500ms) applied to interactive elements to provide visual feedback
- **Hover_Effect**: A visual change triggered by the CSS `:hover` pseudo-class on interactive elements
- **Existing_Colors**: The color palette already defined in the Design_System — specifically `$copper`, `$copper-light`, `$amber-glow`, `$obsidian`, `$velvet-crema`, `$espresso`, and their variants
- **Existing_Fonts**: The typefaces already in use — Playfair Display (serif) and Inter (sans-serif)
- **SCSS_Component_File**: A `.component.scss` file co-located with an Angular component
- **Global_SCSS**: Files under `src/styles/` that apply app-wide

---

## Requirements

### Requirement 1: Warm Card Treatment on Auth Pages

**User Story:** As a visitor to the Ember & Bean site, I want the login, register, and OTP verification cards to feel warm and brand-cohesive, so that the experience matches the premium coffee roastery aesthetic from the first interaction.

#### Acceptance Criteria

1. WHEN the Auth_Pages are rendered, THE UI SHALL display Card backgrounds using dark, warm tones drawn from Existing_Colors (e.g., semi-transparent espresso, dark roast brown, or copper-tinted overlays) rather than white or silver frosted glass.
2. WHEN the Auth_Pages are rendered, THE UI SHALL apply a backdrop blur of at least 16px to Auth_Pages Cards to maintain legibility against the background image.
3. WHEN the Auth_Pages are rendered, THE UI SHALL render Card borders using copper or amber tones from Existing_Colors rather than white or near-white borders.
4. WHEN the Auth_Pages are rendered, THE UI SHALL display all text in Auth_Pages Cards using Existing_Colors (`$velvet-crema` for primary text, `$text-muted` for secondary text) to ensure contrast against the warm dark card background.
5. THE UI SHALL preserve all Existing_Fonts (Playfair Display and Inter) and all Existing_Colors on Auth_Pages without modification.

---

### Requirement 2: Warm Card Treatment on Admin and Client Dashboard Pages

**User Story:** As an admin or client user, I want the dashboard cards, stat panels, and content containers to feel warm and on-brand, so that the interior app experience is consistent with the roastery aesthetic.

#### Acceptance Criteria

1. WHEN Admin_Pages or Client_Pages are rendered, THE UI SHALL display Card backgrounds using the warm dark glassmorphism tokens already defined in the Design_System (`$glass-bg`, `$glass-bg-heavy`, `$glass-border`) rather than neutral or cold backgrounds.
2. WHEN Admin_Pages or Client_Pages are rendered, THE UI SHALL apply copper or amber accent borders (`$glass-border`, `$glass-border-glow`) to Cards on hover or active states.
3. THE UI SHALL ensure that all Card text on Admin_Pages and Client_Pages uses Existing_Colors for legibility against dark warm backgrounds.
4. THE UI SHALL preserve the existing sidebar background treatment (`$glass-bg-heavy` with `$border-subtle`) on both Admin_Pages and Client_Pages without regression.

---

### Requirement 3: Interactive Hover Effects on Cards

**User Story:** As a user browsing the app, I want cards and panels to respond visually when I hover over them, so that the interface feels alive and interactive rather than static.

#### Acceptance Criteria

1. WHEN a user hovers over a Card on any page, THE UI SHALL apply a Hover_Effect that includes at minimum a subtle upward translate (1–3px) and an increase in border glow intensity using Existing_Colors.
2. WHEN a user hovers over a Card on any page, THE UI SHALL complete the Hover_Effect transition within 300ms using the `$ease-smooth` easing token.
3. WHEN a user stops hovering over a Card, THE UI SHALL reverse the Hover_Effect and return the Card to its default state within 300ms.
4. WHEN a user hovers over a navigation item in the sidebar on Admin_Pages or Client_Pages, THE UI SHALL apply a Hover_Effect that changes the item background to `$smoke` and text color to `$velvet-crema`.

---

### Requirement 4: Micro-Animations on Interactive Elements

**User Story:** As a user interacting with buttons, inputs, and links, I want subtle animations that confirm my actions, so that the app feels responsive and premium.

#### Acceptance Criteria

1. WHEN a user focuses an input field on any Auth_Page, THE UI SHALL apply a Micro_Animation that transitions the input border color to a copper or amber Existing_Color and adds a soft glow shadow within 200ms.
2. WHEN a user hovers over a primary CTA button (e.g., Sign In, Register, Submit), THE UI SHALL apply a Micro_Animation that lifts the button by 1–2px and increases the button shadow intensity within 200ms.
3. WHEN a user clicks a primary CTA button, THE UI SHALL apply a Micro_Animation that briefly depresses the button (translate down 1px) within 100ms to confirm the press.
4. WHEN a page or Card first renders, THE UI SHALL apply a fade-in-up entrance animation using the existing `fadeInUp` keyframe (defined in `_animations.scss`) with a duration of 400ms–700ms.
5. WHEN a user hovers over a navigation link or sidebar nav item, THE UI SHALL apply a Micro_Animation that transitions color and background within 150ms.

---

### Requirement 5: Visual Consistency Across All Pages

**User Story:** As a user navigating between auth, admin, and client sections, I want a consistent visual language, so that the app feels like a unified product rather than a patchwork of different styles.

#### Acceptance Criteria

1. THE UI SHALL apply the Warm_Card_Treatment consistently to all Card elements across Auth_Pages, Admin_Pages, and Client_Pages.
2. THE UI SHALL use Design_System tokens (from `_variables.scss` and `_glassmorphism.scss`) for all card backgrounds, borders, shadows, and transition timings — no hardcoded color or timing values outside the Design_System.
3. WHEN a new card style is needed, THE UI SHALL reuse or extend existing Design_System mixins (`glass-card`, `glass-card-hover`, `glass-panel`) rather than defining isolated one-off styles.
4. THE UI SHALL maintain the existing responsive breakpoints (`$bp-mobile: 480px`, `$bp-tablet: 768px`) and ensure Warm_Card_Treatment and Hover_Effects are applied correctly at all breakpoints.
5. IF a component's SCSS file defines local color variables that duplicate Design_System tokens, THEN THE UI SHALL replace those local variables with the corresponding Design_System tokens.

---

### Requirement 6: No Backend or Logic Changes

**User Story:** As a developer maintaining the app, I want the redesign to be strictly frontend-only, so that no backend services, API contracts, or Angular component logic are affected.

#### Acceptance Criteria

1. THE UI SHALL limit all redesign changes to SCSS_Component_Files and Global_SCSS files only.
2. THE UI SHALL not modify any Angular component TypeScript (`.component.ts`) files as part of this redesign.
3. THE UI SHALL not modify any HTML template (`.component.html`) files unless a change is strictly required to add a CSS class that enables a visual effect with no logic change.
4. THE UI SHALL not modify any backend service, API, or microservice files.
5. IF an HTML template change is made, THEN THE UI SHALL limit the change to adding or modifying CSS class attributes only, with no changes to Angular bindings, directives, event handlers, or component logic.

---

### Requirement 7: Accessibility and Legibility Preservation

**User Story:** As a user with varying visual needs, I want the redesigned warm dark cards to remain legible and accessible, so that I can use the app comfortably.

#### Acceptance Criteria

1. WHEN the Warm_Card_Treatment is applied, THE UI SHALL maintain a minimum contrast ratio of 4.5:1 between body text and Card backgrounds, in accordance with WCAG 2.1 AA guidelines.
2. WHEN the Warm_Card_Treatment is applied, THE UI SHALL maintain a minimum contrast ratio of 3:1 between large text (headings ≥ 18px or bold ≥ 14px) and Card backgrounds.
3. WHEN Micro_Animations are applied, THE UI SHALL respect the `prefers-reduced-motion` media query by disabling or reducing animations for users who have enabled reduced motion in their OS settings.
4. WHEN input focus states are styled, THE UI SHALL ensure the focus indicator remains clearly visible (minimum 2px outline or equivalent glow) to support keyboard navigation.
