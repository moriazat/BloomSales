﻿using BloomSales.Data.Entities;
using BloomSales.Services.Contracts;
using BloomSales.Services.Proxies;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BloomSales.Web.Store.Controllers
{
    public class ShoppingCartController : Controller
    {
        private IOrderService orderService;
        private IInventoryService inventoryService;

        public ShoppingCartController()
        {
            orderService = new OrderClient();
            inventoryService = new InventoryClient();
        }

        // GET: ShoppingCart
        public ActionResult Index()
        {
            var order = GetShoppingCart();
            var productIDs = new List<int>();

            foreach (var item in order.Items)
                productIDs.Add(item.ProductID);

            var products = inventoryService.GetProductsByIDs(productIDs);
            var items = new List<OrderItem>(order.Items);

            Tuple<List<OrderItem>, IEnumerable<Product>> cartItems =
                new Tuple<List<OrderItem>, IEnumerable<Product>>(items, products);

            return View(cartItems);
        }

        public ActionResult Count()
        {
            Order order = null;

            order = GetShoppingCart();

            if (order == null)
                return PartialView(0);

            int count = order.Items.Count();
            return PartialView(count);
        }

        [HttpPost]
        public ActionResult Add(int productItemID, decimal unitPrice)
        {
            Order order;
            List<OrderItem> items;
            OrderItem item = new OrderItem();
            item.ProductID = productItemID;
            item.Quantity = 1;
            item.UnitPrice = unitPrice;

            order = GetShoppingCart();
            items = GetItems(ref order);

            AddItemToList(items, item);

            order.Items = items;

            SetShoppingCart(order);

            return RedirectToAction("Count");
        }

        [HttpPost]
        public ActionResult Update(List<OrderItem> items)
        {
            List<OrderItem> updatedItemsList = new List<OrderItem>();

            foreach (OrderItem item in items)
                if (item.Quantity > 0)
                    updatedItemsList.Add(item);

            var order = GetShoppingCart();

            order.Items = updatedItemsList;

            SetShoppingCart(order);

            return RedirectToAction("Calculate");
        }

        public ActionResult Calculate()
        {
            var order = GetShoppingCart();
            decimal subtotal = 0;

            foreach (var item in order.Items)
                subtotal += item.Quantity * item.UnitPrice;

            return PartialView(subtotal);
        }

        public ActionResult CheckoutTransition()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Checkout");
            else
                return RedirectToAction("SaveCart");
        }

        [Authorize]
        public ActionResult SaveCart()
        {
            Order order = GetCartFromSession();

            SetShoppingCart(order);

            DeleteCartFromSession();

            return RedirectToAction("Index", "Checkout");
        }

        private void SetShoppingCart(Order order)
        {
            if (User.Identity.IsAuthenticated)
                orderService.AddOrUpdateCart(User.Identity.GetUserId(), order);
            else
                SetCartInSession(order);
        }

        private List<OrderItem> GetItems(ref Order order)
        {
            List<OrderItem> items;

            if (order == null)
            {
                order = new Order();
                items = new List<OrderItem>();
            }
            else
            {
                items = new List<OrderItem>(order.Items);
            }

            return items;
        }

        private Order GetShoppingCart()
        {
            Order order;
            if (User.Identity.IsAuthenticated)
                order = orderService.GetCart(User.Identity.GetUserId());
            else
                order = GetCartFromSession();
            return order;
        }

        private void AddItemToList(List<OrderItem> items, OrderItem item)
        {
            var duplicate = items.Find(i => i.ProductID == item.ProductID);

            if (duplicate != null)
                duplicate.Quantity++;
            else
                items.Add(item);
        }

        private Order GetCartFromSession()
        {
            return Session["cart"] as Order;
        }

        private void SetCartInSession(Order order)
        {
            Session["cart"] = order;
        }

        private void DeleteCartFromSession()
        {
            Session["cart"] = null;
        }
    }
}