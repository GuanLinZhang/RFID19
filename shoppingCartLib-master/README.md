# shoppingCartLib 

Overall Explanation

This shopping cart control is designed for cashers to place an order in a fast-food restaurant, such as KFC, McDonalds etc. It is ideally used on a touch screen. It has user-friendly interface, with clear instructions and reasonable size icons and buttons. The category filter and search product by code/name functions are helpful to find products quickly. Cashiers can also modify the quantity of products, delete a product and apply coupon discount to specific products or categories. Total price will be calculated automatically once any change is applied to an order. This control is not connected to database at this stage, all the products and coupons information are from an external XML file and order details will be saved to an external XML file as well. Connection to database would be done if required.

Features

Primary Features:
         
1.	When the user clicks on the item, the Add and Subtract buttons will appear to increase or decrease the quantity of the item clicked respectively. The net price, tax and total price in order summary area change simultaneously.

2.	When user enters coupon code and clicks APPLY COUPON button, system will check the validity and applicable items or category. If this coupon is valid, it will apply to the corresponding items, and display discount rate and discounted price.

Secondary Features:

1.	The cashier can search items by item code or name, the search result appears in the item display area simultaneously. 

2.	When clicking the Clear Filter button, the system ignores the category and search filter and display all items.

3.	The cashier can cancel the order if required, all items in the current order will be deleted and a new order with new order number will be generated.

4.	When the cashier clicks the PAY button, a receipt will be displayed. Once cashier confirms the payment, order details will be added to the external XML file.



Here below is the screenshot: 

![alt tag](https://github.com/TaoL102/shoppingCartLib/blob/master/Test/screenshot.png)
