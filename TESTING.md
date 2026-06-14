# LightMarket Testing Checklist

This document contains manual test cases for all major features of the LightMarket application.

## Authentication Module

### Send OTP
- [ ] Enter valid phone number (09123456789)
- [ ] Verify OTP is received via SMS
- [ ] Verify OTP expires after 2 minutes
- [ ] Test with invalid phone number format
- [ ] Test rate limiting (max 3 requests per minute)

### Verify OTP
- [ ] Enter correct OTP code
- [ ] Verify JWT token is returned
- [ ] Verify token is stored in localStorage
- [ ] Test with incorrect OTP code
- [ ] Test with expired OTP code

### Register/Login
- [ ] New user registration creates account
- [ ] Existing user can login
- [ ] User profile is populated correctly

## Campaign Module

### Get Active Campaign
- [ ] Returns default active campaign
- [ ] Campaign data includes all required fields
- [ ] Handles case when no active campaign exists

### Get Campaign by Slug
- [ ] Valid slug returns correct campaign
- [ ] Invalid slug returns 404
- [ ] URL-friendly slugs work correctly

### Get Campaign Products
- [ ] Returns products for given campaign
- [ ] Products include price, discount, stock
- [ ] Out of stock products are marked correctly

## Cart Module

### Add to Cart
- [ ] Product added successfully
- [ ] Quantity validation (min/max limits)
- [ ] Stock availability check
- [ ] Campaign active status check
- [ ] Cart created if not exists

### Update Quantity
- [ ] Quantity updated correctly
- [ ] Cannot exceed stock available
- [ ] Cannot go below minimum quantity
- [ ] Price recalculated automatically

### Remove from Cart
- [ ] Item removed successfully
- [ ] Cart totals updated
- [ ] Empty cart handled gracefully

### Get Cart
- [ ] Returns cart with all items
- [ ] Totals calculated correctly (subtotal, discount, tax)
- [ ] Returns empty cart for new users

## Checkout Module

### Step 1: Address Selection
- [ ] User addresses loaded correctly
- [ ] Can select existing address
- [ ] Can add new address
- [ ] Address validation works

### Step 2: Shipping Method
- [ ] Shipping options displayed
- [ ] Shipping cost calculated correctly
- [ ] Different methods have different costs

### Step 3: Final Review
- [ ] Invoice summary shows all items
- [ ] Totals match cart totals
- [ ] Discount applied correctly
- [ ] Tax calculated correctly
- [ ] Shipping cost included

### Create Order
- [ ] Order created with status "PaymentPending"
- [ ] Stock held temporarily
- [ ] Order confirmation sent via SMS

## Payment Module

### Create Payment Request
- [ ] Payment gateway URL returned
- [ ] Amount matches order total
- [ ] Callback URL configured correctly

### Verify Payment
- [ ] Successful payment updates order status
- [ ] Failed payment keeps order pending
- [ ] SMS notifications sent

### Magic Link
- [ ] Link generated with correct expiry
- [ ] Link works without login
- [ ] Expired link shows error
- [ ] Single-use token invalidated after use

## Order Management

### View Orders
- [ ] User sees their orders only
- [ ] Order list shows key information
- [ ] Order details page loads correctly

### Order Status Updates
- [ ] PaymentConfirmed status after payment
- [ ] Shipped status after shipping update
- [ ] Tracking info visible to user

### Edit Invoice (Admin)
- [ ] Changes logged to audit log
- [ ] Totals recalculated
- [ ] Refund/charge difference tracked
- [ ] Reason for change recorded

## Support Tickets

### Create Ticket
- [ ] Ticket created successfully
- [ ] Category selected correctly
- [ ] Confirmation message shown

### View Tickets
- [ ] User sees their tickets only
- [ ] Ticket list shows status and priority
- [ ] Ticket detail shows conversation thread

### Reply to Ticket
- [ ] Reply added to thread
- [ ] Admin can reply
- [ ] User notified of admin reply
- [ ] Closed tickets cannot be replied to

## Admin Panel

### Campaign Management
- [ ] Create new campaign
- [ ] Update existing campaign
- [ ] View campaign performance report
- [ ] Activate/deactivate campaigns

### Product Import (Excel)
- [ ] Excel file uploaded
- [ ] Fuzzy matching suggests correct products
- [ ] Confidence score displayed
- [ ] Manual confirmation for low confidence
- [ ] Products imported successfully

### Price Scraping
- [ ] Scraper configured for supplier
- [ ] Prices updated from supplier website
- [ ] Price changes logged
- [ ] Failed scrapes handled gracefully

## UI/UX Tests

### Responsive Design
- [ ] Mobile layout (≤576px) - single column
- [ ] Tablet layout (577-992px) - two columns
- [ ] Desktop layout (≥993px) - three columns
- [ ] Navigation menu adapts correctly
- [ ] Forms usable on all screen sizes

### Loading States
- [ ] Spinner shown during API calls
- [ ] Buttons disabled during submission
- [ ] Skeleton loaders for content

### Error Handling
- [ ] 404 page displays correctly
- [ ] 500 page displays correctly
- [ ] Network errors shown to user
- [ ] Form validation errors clear and helpful

### RTL Support
- [ ] All text aligned right-to-left
- [ ] Icons and arrows flipped correctly
- [ ] Numbers display correctly (Persian/English)

## Performance Tests

- [ ] Page load time < 3 seconds
- [ ] API response time < 500ms
- [ ] Images optimized and lazy-loaded
- [ ] No memory leaks during extended use

## Security Tests

- [ ] JWT tokens expire correctly
- [ ] Protected routes require authentication
- [ ] Users can only access their own data
- [ ] SQL injection prevented
- [ ] XSS attacks prevented
- [ ] CSRF protection enabled

## Browser Compatibility

- [ ] Chrome (latest)
- [ ] Firefox (latest)
- [ ] Safari (latest)
- [ ] Edge (latest)
- [ ] Mobile browsers (iOS Safari, Chrome Mobile)
