using DoAnAPI.IRepository;
using DoAnAPI.Models.Domain;
using DoAnAPI.Models.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace DoAnAPI.Repository
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly DatabaseContext _context;

        public InvoiceRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<Result<List<InvoiceDTO>>> GetAllInvoices()
        {
            // Lấy danh sách hóa đơn từ cơ sở dữ liệu
            try
            {
                var invoiceList = await _context.Invoices.ToListAsync();
                var invoiceDTOList = new List<InvoiceDTO>();
                foreach( var invoice in invoiceList)
                {
                    var InvoiceDTO = new InvoiceDTO
                    {
                        
                        Id =invoice.Id,
                        UserId = invoice.UserId,
                        Date = invoice.Date,
                        Price = invoice.TotalAmount,
                        InvoiceProducts = new List<InvoiceProductDTO>()
                    };
                    foreach (var i in invoice.InvoiceProducts)
                    {
                        var invoiceProductDTO = new InvoiceProductDTO
                        {
                            ProductId = i.ProductId,
                            Product = i.Product,
                            Quantity = i.Quantity,
                            Price = i.Price,
                        };
                        InvoiceDTO.InvoiceProducts.Add(invoiceProductDTO);
                    }
                    invoiceDTOList.Add(InvoiceDTO);
                }
                return Result<List<InvoiceDTO>>.Success(invoiceDTOList);
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và trả về kết quả thất bại
                return Result<List<InvoiceDTO>>.Fail(ex.Message);
            }


        }

        public async Task<Result<InvoiceDTO>> GetInvoiceById(int? id)
        {
            try
            {
                if (id == null)
                {
                    return Result<InvoiceDTO>.Fail("Invalid invoice ID passed to GetInvoiceById");
                }

                // Lấy hóa đơn theo ID từ cơ sở dữ liệu
                var invoice = await _context.Invoices
                    .Include(i => i.InvoiceProducts) // Kèm theo danh sách InvoiceProducts
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (invoice != null)
                {
                    var result = new InvoiceDTO
                    {
                        Price = invoice.TotalAmount,
                        Date = invoice.Date,
                        UserId = invoice.UserId,
                        InvoiceProducts = new List<InvoiceProductDTO>()
                    };

                    foreach (var i in invoice.InvoiceProducts)
                    {
                        var invoiceProductDTO = new InvoiceProductDTO
                        {
                            Price = i.Price,
                            ProductId = i.ProductId,
                            Product = i.Product,
                            Quantity = i.Quantity
                        };
                        result.InvoiceProducts.Add(invoiceProductDTO);
                    }
                    return Result<InvoiceDTO>.Success(result);
                }
                else
                {
                    return Result<InvoiceDTO>.Fail("Invoice not found.");
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và trả về kết quả thất bại
                return Result<InvoiceDTO>.Fail(ex.Message);
            }
        }

        /*   public async Task<Result<InvoiceDTO>> CreateInvoiceAsync(InvoiceDTO invoiceDataTrans)
           {
               try
               {
                   // Tạo đối tượng hóa đơn từ DTO
                   var invoice = new Invoice
                   {
                       Date = invoiceDataTrans.Date,
                       CustomerId = invoiceDataTrans.CustomerId,
                       InvoiceProducts = new List<InvoiceProduct>()
                   };

                   // Kiểm tra số lượng hàng tồn kho trước khi tạo hóa đơn
                   foreach (var productDTO in invoiceDataTrans.InvoiceProducts)
                   {
                       var product = await _context.Products.FindAsync(productDTO.ProductId);
                       if (product == null)
                       {
                           return Result<InvoiceDTO>.Fail($"Product with ID {productDTO.ProductId} not found.");
                       }

                       if (product.Quantity < productDTO.Quantity)
                       {
                           return Result<InvoiceDTO>.Fail($"Not enough quantity for product with ID {productDTO.ProductId}.");
                       }
                       product.Quantity -= productDTO.Quantity;
                   }

                   // Thêm hóa đơn vào cơ sở dữ liệu
                   var result = await AddInvoice(invoice);
                   if (result.IsSuccess)
                   {
                       var newInvoice = result.Data; // Lấy hóa đơn từ kết quả thành công
                       foreach (var productDTO in invoiceDataTrans.InvoiceProducts)
                       {
                           var invoiceProduct = new InvoiceProduct
                           {
                               InvoiceId = (int)result.Data.Id,
                               ProductId = productDTO.ProductId,
                               Quantity = productDTO.Quantity
                           };
                           invoice.InvoiceProducts.Add(invoiceProduct); // Thêm vào danh sách hóa đơn
                           _context.InvoiceProducts.Add(invoiceProduct); // Thêm vào DbContext
                       }
                       await _context.SaveChangesAsync();
                   }
                   else
                   {
                       return Result<InvoiceDTO>.Fail(result.Message);
                   }

                   // Trả về kết quả thành công và thông tin hóa đơn
                   result = await GetInvoiceById(result.Data.Id);
                   return result;
               }
               catch (Exception ex)
               {
                   // Xử lý các lỗi và trả về kết quả thất bại nếu cần
                   return Result<InvoiceDTO>.Fail(ex.Message);
               }
           }*/
        public async Task<Result<Invoice>> CreateInvoiceAsync(Invoice invoice, List<ProductCartVm> productCart)
        {
            try
            {
                foreach (var productDTO in productCart)
                {
                    var product = await _context.Products.FindAsync(productDTO.ProductId);
                    if (product == null)
                    {
                        return Result<Invoice>.Fail($"Product with ID {productDTO.ProductId} not found.");
                    }

                    if (product.Quantity < productDTO.OrderQuanity)
                    {
                        return Result<Invoice>.Fail($"Not enough quantity for product with ID {productDTO.ProductId}.");
                    }
                    product.Quantity -= productDTO.OrderQuanity;
                }
                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();
                foreach (var product in productCart)
                {
                    var invoiceProduct = new InvoiceProduct
                    {
                        ProductId = product.ProductId,
                        InvoiceId = invoice.Id,
                        Quantity = product.OrderQuanity,
                        Price = product.Price
                        

                    };
                    _context.InvoiceProducts.Add(invoiceProduct);
                    await _context.SaveChangesAsync();
                }

                return Result<Invoice>.Success(invoice);
            }
            catch (Exception ex)
            {
                // Log exception if needed
                return Result<Invoice>.Fail("Failed to create invoice: " + ex.Message);
            }
        }

        public async Task<Result<InvoiceDTO>> AddInvoice(Invoice invoice)
        {
            try
            {
                // Thêm hóa đơn mới vào cơ sở dữ liệu
                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();

                // Tạo đối tượng DTO từ đối tượng Invoice
                var invoiceDTO = new InvoiceDTO
                {
                    Id = invoice.Id,
                    UserId = invoice.UserId,
                    Date = invoice.Date,
                    Price = invoice.TotalAmount
                };

                return Result<InvoiceDTO>.Success(invoiceDTO);
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và trả về kết quả thất bại
                return Result<InvoiceDTO>.Fail(ex.Message);
            }
        }


        public async Task<Result<Invoice>> UpdateInvoice(Invoice invoice)
        {
            // Cập nhật thông tin hóa đơn trong cơ sở dữ liệu
            _context.Entry(invoice).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return Result<Invoice>.Success(invoice);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvoiceExists(invoice.Id))
                    return Result<Invoice>.Fail("Invoice not found.");
                else
                    throw;
            }
        }

        public async Task<Result<InvoiceDTO>> DeleteInvoice(int id)
        {
            try
            {
                // Tìm hóa đơn để xóa từ cơ sở dữ liệu
                var invoice = await _context.Invoices.FindAsync(id);
                if (invoice == null)
                    return Result<InvoiceDTO>.Fail("Invoice not found.");

                // Xóa các sản phẩm hóa đơn liên quan trước khi xóa hóa đơn
                _context.InvoiceProducts.RemoveRange(invoice.InvoiceProducts);

                // Xóa hóa đơn từ cơ sở dữ liệu
                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();

                // Trả về kết quả thành công với hóa đơn đã xóa
                var invoiceDTO = new InvoiceDTO
                {   
                    Price = invoice.TotalAmount,
                    Id = invoice.Id,
                    UserId = invoice.UserId,
                    Date = invoice.Date
                };
                return Result<InvoiceDTO>.Success(invoiceDTO);
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và trả về kết quả thất bại
                return Result<InvoiceDTO>.Fail(ex.Message);
            }
        }
        public async Task<Result<List<InvoiceDTO>>> GetInvoicesByCustomerId(string customerId)
        {
            try
            {
                // Lấy danh sách hóa đơn từ cơ sở dữ liệu dựa trên ID của khách hàng
                var invoices = await _context.Invoices
                    .Where(i => i.UserId== customerId)
                    .Include(i => i.InvoiceProducts)
                    .ToListAsync();

                // Chuyển đổi danh sách hóa đơn sang danh sách DTO
                var invoiceDTOs = invoices.Select(invoice => new InvoiceDTO
                {
                    Price = invoice.TotalAmount,
                    Id = invoice.Id,
                    Date = invoice.Date,
                    UserId = invoice.UserId,
                    InvoiceProducts = invoice.InvoiceProducts.Select(ip => new InvoiceProductDTO
                    {
                        Price = ip.Price,
                        Product = ip.Product,
                        Quantity = ip.Quantity
                    }).ToList()
                }).ToList();

                return Result<List<InvoiceDTO>>.Success(invoiceDTOs);
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và trả về kết quả thất bại nếu cần
                return Result<List<InvoiceDTO>>.Fail(ex.Message);
            }
        }


        private bool InvoiceExists(int id)
        {
            // Kiểm tra xem hóa đơn có tồn tại trong cơ sở dữ liệu không
            return _context.Invoices.Any(e => e.Id == id);
        }
    }
}
