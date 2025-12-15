# Frontend Password Show/Hide Guidelines

## Overview
Pre-Clear frontend uses an eye icon toggle to let users reveal or mask passwords. This improves UX on mobile and desktop while maintaining security best practices.

## Component: PasswordInput

A reusable React component is available at `src/components/ui/PasswordInput.jsx`. It encapsulates password field best practices:

```jsx
import { PasswordInput } from '../components/ui/PasswordInput';

export function MyForm() {
  const [password, setPassword] = useState('');
  const [errors, setErrors] = useState({});

  return (
    <PasswordInput
      value={password}
      onChange={setPassword}
      label="Password"
      placeholder="Enter your password"
      error={errors.password}
      required
    />
  );
}
```

## UX Guidelines

### Eye Icon Placement
- **Position**: Inside the input field, aligned to the **right**.
- **Margin**: 16px from the right edge (pr-12 in Tailwind = 48px padding-right, button at `right-4`).
- **Size**: 20px × 20px (w-5 h-5 in Tailwind).
- **Icons**: Use lucide-react `Eye` and `EyeOff` for consistency.

### Visual Styling
- **Background**: Transparent (no button background).
- **Border**: None (button sits inside input).
- **Color**: Match input text color (e.g., `#7A5B52` soft coffee in Pre-Clear).
- **Hover/Focus**: Subtle color change or opacity shift (optional).
- **Disabled State**: Opacity 0.6 when input is disabled.

### Example CSS (Tailwind + Inline)
```jsx
<button
  className="absolute right-4 focus:outline-none focus:ring-2 rounded"
  style={{
    background: 'transparent',
    border: 'none',
    cursor: 'pointer',
    padding: '4px',
    display: 'flex',
    alignItems: 'center'
  }}
>
  {showPassword ? <EyeOff /> : <Eye />}
</button>
```

## Accessibility (A11y)

### aria-label
Always provide a human-readable label for the toggle button:
```jsx
aria-label={showPassword ? 'Hide password' : 'Show password'}
```

### aria-pressed
Indicate the button's toggle state:
```jsx
aria-pressed={showPassword}
```

### Keyboard Navigation
- **Tab**: Focus the button normally.
- **Enter/Space**: Toggle password visibility.
- **Tab out**: Move to the next focusable element.

### Screen Reader Behavior
- Screen readers announce: "Toggle password visibility, button, not pressed" (or "pressed" if active).
- Users hear the label when they tab to the button.

### Focus Ring
Use browser default or custom focus ring:
```jsx
className="focus:outline-none focus:ring-2 focus:ring-offset-0"
```

## Implementation Examples

### In SignUp Form
```jsx
import { PasswordInput } from './ui/PasswordInput';

export function SignupPage() {
  const [formData, setFormData] = useState({
    password: '',
    confirmPassword: ''
  });

  return (
    <>
      <PasswordInput
        value={formData.password}
        onChange={(val) => setFormData((p) => ({ ...p, password: val }))}
        label="Password"
        placeholder="Min. 8 characters"
        error={errors.password}
        required
      />
      <PasswordInput
        value={formData.confirmPassword}
        onChange={(val) => setFormData((p) => ({ ...p, confirmPassword: val }))}
        label="Confirm Password"
        placeholder="Re-enter password"
        error={errors.confirmPassword}
        required
      />
    </>
  );
}
```

### Submitting ShowPassword Flag (Optional)

When submitting a form, you may include the `showPassword` flag if desired, but it is **optional**:

```jsx
const handleSubmit = async (e) => {
  e.preventDefault();

  const payload = {
    firstName: formData.firstName,
    password: formData.password,
    confirmPassword: formData.confirmPassword,
    // Optional: include showPassword for tracking/analytics (server ignores it)
    showPassword: showPasswordState
  };

  await signUp(payload);
};
```

**Important**: The server will ignore this flag entirely. It is provided purely for logging or analytics on the client side if desired.

## Security Best Practices

### What to DO
✅ Store `showPassword` state in React component state only.  
✅ Clear password fields after successful form submission.  
✅ Use HTTPS to encrypt passwords in transit.  
✅ Validate passwords on both client and server.  
✅ Use the password input's native browser autocomplete (`autocomplete="password"`).  
✅ Use `type="password"` when hiding; switch to `type="text"` only when user toggles show.  

### What NOT to DO
❌ Store passwords in localStorage, sessionStorage, or cookies.  
❌ Store the `showPassword` flag in localStorage (it will survive page reload and leak intent).  
❌ Log passwords to the console in development or production.  
❌ Include passwords in error messages or alerts.  
❌ Send passwords in query parameters or URL fragments.  
❌ Disable browser password managers (use `autocomplete="current-password"` and let browser help).  

## Example: Persist ShowPassword Locally (Optional)

If you want to remember the user's toggle preference across page loads, you can store it **separately** (not the password):

```jsx
const [showPassword, setShowPassword] = useState(
  () => sessionStorage.getItem('pwdToggle') === 'true'
);

const handleToggle = () => {
  const newState = !showPassword;
  setShowPassword(newState);
  // Store preference in session (cleared on browser close)
  sessionStorage.setItem('pwdToggle', newState);
};
```

**Important**: Only store the **toggle preference**, not the password value.

## Testing Checklist

- [ ] Eye icon is visible and correctly positioned inside the input.
- [ ] Clicking the eye toggles between `type="password"` and `type="text"`.
- [ ] Icon changes from Eye to EyeOff when password is shown.
- [ ] Button is keyboard-focusable (Tab key works).
- [ ] Button can be activated with Enter or Space.
- [ ] `aria-label` is announced by screen readers.
- [ ] `aria-pressed` state changes correctly.
- [ ] Passwords are NOT logged to the console.
- [ ] Passwords are NOT stored in localStorage.
- [ ] Form submission includes optional `showPassword` flag (if desired).
- [ ] Server ignores the `showPassword` flag.
- [ ] Server never returns passwords or `showPassword` in responses.

## Browser Compatibility

- **Modern browsers**: Full support (Chrome, Firefox, Safari, Edge).
- **IE 11**: Requires polyfills for Eye/EyeOff icons from lucide-react.
- **Mobile browsers**: All supported; eye icon is easily tappable (minimum 44px height).

## References

- [WCAG 2.1 - Button Controls](https://www.w3.org/WAI/WCAG21/Understanding/headings-and-labels)
- [MDN - Password Input Type](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input/password)
- [OWASP - Authentication Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Authentication_Cheat_Sheet.html)
- [Lucide React Icons](https://lucide.dev/)
